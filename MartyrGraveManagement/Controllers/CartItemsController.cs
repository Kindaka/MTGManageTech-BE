using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using Microsoft.AspNetCore.Authorization;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartService _cartItemsService;
        private readonly IAuthorizeService _authorizeService;

        public CartItemsController(ICartService cartItemsService, IAuthorizeService authorizeService)
        {
            _cartItemsService = cartItemsService;
            _authorizeService = authorizeService;
        }

        // GET: api/CartItems
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemsDTOResponse>>> GetCartItems()
        {
            var cartItems = await _cartItemsService.GetAllCartItems();
            return Ok(cartItems);
        }

        // GET: api/CartItems/5
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItemsDTOResponse>> GetCartItem(int id)
        {
            var cartItem = await _cartItemsService.GetAllCartItemById(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return Ok(cartItem);
        }


        // POST: api/CartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<ActionResult> CreateCartItems(List<CartItemsDTORequest> cartItemsDTOs)
        {
            try
            {
                // Lấy AccountId từ thông tin đăng nhập của người dùng
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }

                // Kiểm tra tất cả các mục trong danh sách có hợp lệ với AccountId của người dùng không
                foreach (var cartItemDTO in cartItemsDTOs)
                {
                    var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(cartItemDTO.AccountId, int.Parse(accountId));
                    if (!checkMatchedId.isMatchedCustomer)
                    {
                        return Forbid();
                    }

                    if (cartItemDTO == null)
                    {
                        return BadRequest("Cannot add empty object to cart");
                    }
                }

                // Gọi service để tạo danh sách CartItems và lấy kết quả
                var (responses, messages) = await _cartItemsService.CreateCartItemsAsync(cartItemsDTOs);

                // Trả về kết quả bao gồm danh sách phản hồi và thông báo
                return Ok(new { responses, messages });
            }
            catch (KeyNotFoundException ex)
            {
                // Xử lý lỗi không tìm thấy dữ liệu
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Xử lý lỗi liên quan đến logic kinh doanh (vd: GraveService không tồn tại)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi không mong muốn
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }





        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("cart/{customerId}")]
        public async Task<ActionResult<List<CartItemGetByCustomerDTOResponse>>> GetCartItemsByAccountId(int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }
                // Gọi service để lấy giỏ hàng của accountId
                var cartItems = await _cartItemsService.GetCartItemsByAccountId(customerId);

                // Kiểm tra nếu không có giỏ hàng nào được tìm thấy
                if (cartItems.cartitemList == null || !cartItems.cartitemList.Any())
                {
                    return Ok(cartItems);
                }

                // Trả về danh sách các mục trong giỏ hàng
                return Ok(new { cartItemList = cartItems.cartitemList, totalPrice = cartItems.totalPriceInCart});
            }
            catch (Exception ex)
            {
                // Quản lý lỗi nếu có ngoại lệ xảy ra
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("checkout/{customerId}")]
        public async Task<ActionResult<List<CartItemGetByCustomerDTOResponse>>> GetCheckoutByAccountId(int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }

                var cartItems = await _cartItemsService.GetCheckoutByAccountId(customerId);

                
                if (cartItems.cartitemList == null || !cartItems.cartitemList.Any())
                {
                    return Ok(cartItems);
                }

                // Trả về danh sách các mục trong giỏ hàng
                return Ok(new { cartItemList = cartItems.cartitemList, totalPrice = cartItems.totalPriceInCart });
            }
            catch (Exception ex)
            {
                // Quản lý lỗi nếu có ngoại lệ xảy ra
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemInCart(int id)
        {
            try
            {
                var customerId = User.FindFirst("AccountId")?.Value;
                if (customerId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCartId(id, int.Parse(customerId));
                if (!checkMatchedId)
                {
                    return Forbid();
                }
                var check = await _cartItemsService.DeleteCartItemsAsync(id);
                if (check)
                {
                    return Ok("Delete successfully");
                }
                else
                {
                    return BadRequest("Item does not exist");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("/api/updateItemStatus/{cartItemId}/{status}")]
        public async Task<IActionResult> UpdateCartItemStatus(int cartItemId, bool status)
        {
            try
            {
                var customerId = User.FindFirst("AccountId")?.Value;
                if (customerId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCartId(cartItemId, int.Parse(customerId));
                if (!checkMatchedId)
                {
                    return Forbid();
                }
                var check = await _cartItemsService.UpdateCartItemStatusByAccountId(cartItemId, status);
                if (check)
                {
                    return Ok("Thay đổi trạng thái Cart Item thành công");
                }
                else
                {
                    return BadRequest("Không tìm thấy Item");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get service and martyrGrave information for anonymous guest in cart.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("service-martyrGrave/anonymous/cart")]
        public async Task<IActionResult> GetServiceMartyrGraveInCart(List<ServiceMartyrGraveDtoRequest> request)
        {
            try
            {
                var cartList = await _cartItemsService.GetCartForGuest(request);
                if (cartList.cartitemList.Any())
                {
                    return Ok(new { cartItemList = cartList.cartitemList, totalPrice = cartList.totalPriceInCart });
                }
                else
                {
                    return Ok(cartList);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
