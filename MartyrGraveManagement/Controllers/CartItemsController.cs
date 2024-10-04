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

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartService _cartItemsService;

        public CartItemsController(ICartService cartItemsService)
        {
            _cartItemsService = cartItemsService;
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemsDTOResponse>>> GetCartItems()
        {
            var cartItems = await _cartItemsService.GetAllCartItems();
            return Ok(cartItems);
        }

        // GET: api/CartItems/5
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
        [HttpPost]
        public async Task<ActionResult<CartItemsDTOResponse>> CreateCartItems(CartItemsDTORequest cartItemDTO)
        {
            try
            {
                var createCartItem = await _cartItemsService.CreateCartItemsAsync(cartItemDTO);
                return CreatedAtAction(nameof(GetCartItem), new { id = createCartItem.CartId }, createCartItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

     

        [HttpGet("cart/{accountId}")]
        public async Task<ActionResult<List<CartItemGetByCustomerDTOResponse>>> GetCartItemsByAccountId(int accountId)
        {
            try
            {
                // Gọi service để lấy giỏ hàng của accountId
                var cartItems = await _cartItemsService.GetCartItemsByAccountId(accountId);

                // Kiểm tra nếu không có giỏ hàng nào được tìm thấy
                if (cartItems == null || !cartItems.Any())
                {
                    return NotFound(new { message = "No cart items found for this account." });
                }

                // Trả về danh sách các mục trong giỏ hàng
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                // Quản lý lỗi nếu có ngoại lệ xảy ra
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemInCart(int id)
        {
            try
            {
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

    }
}
