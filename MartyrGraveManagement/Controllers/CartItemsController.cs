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

        // PUT: api/CartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, CartItemsDTORequest cartItemDTO)
        {
            try
            {
                var updateCartItem = await _cartItemsService.UpdateCartItemsAsync(id, cartItemDTO);

          
                if (updateCartItem == null)
                {
                    return NotFound();
                }

                return Ok("Update Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
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

        // DELETE: api/CartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var deleted = await _cartItemsService.DeleteCartItemsAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok("Delete Successfully");
        }

      
    }
}
