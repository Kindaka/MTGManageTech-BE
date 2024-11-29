using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using Microsoft.AspNetCore.Authorization;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _odersService;
        private readonly IAuthorizeService _authorizeService;

        public OrdersController(IOrdersService odersService, IAuthorizeService authorizeService)
        {
            _odersService = odersService;
            _authorizeService = authorizeService;
        }

<<<<<<< Updated upstream
        [Authorize(Policy = "RequireManagerRole")]
=======

        /// <summary>
        /// Gets all martyr graves.
        /// </summary>
        [Authorize(Policy = "RequireManagerOrAdminRole")]
>>>>>>> Stashed changes
        [HttpGet]
        public async Task<ActionResult<List<OrdersGetAllDTOResponse>>> GetAllOrders()
        {
            try
            {
                var orders = await _odersService.GetAllOrders();

                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "No orders found." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersGetAllDTOResponse>> GetOrderById(int id)
        {
            try
            {
                var order = await _odersService.GetOrderById(id);

                if (order == null)
                {
                    return NotFound(new { message = "Order not found." });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }




        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<List<OrdersGetAllDTOResponse>>> GetOrderByAccountId(int customerId)
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
                var orders = await _odersService.GetOrderByAccountId(customerId);

                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "No orders found for this account." });
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<ActionResult<OrdersDTOResponse>> Create(int customerId)
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
                var create = await _odersService.CreateOrderFromCartAsync(customerId);
                if (create.status)
                {
                    return Ok(new { paymentUrl = create.paymentUrl, responseContent = create.responseContent });
                }
                else
                {
                    return BadRequest(create.responseContent);
                }
                
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpPut("/api/v1/updateStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromQuery] int id, [FromQuery] int status)
        {
            try
            {
                var updated = await _odersService.UpdateOrderStatus(id, status);

                if (updated)
                {
                    return Ok(new { message = "Order status updated successfully." });
                }

                return BadRequest(new { message = "Failed to update order status." });
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

    }
}
