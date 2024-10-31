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


        /// <summary>
        /// Gets all martyr graves.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersGetAllDTOResponse>>> GetAllOrders()
        {
            try
            {
                var orders = await _odersService.GetAllOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersGetAllDTOResponse>> GetOrderById(int id)
        {
            try
            {
                var order = await _odersService.GetOrderById(id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }




        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("account/{customerId}")]
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
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }



        /// <summary>
        /// Get orders by area ID (Manager or Staff Role).
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("orders/area/{areaId}")]
        public async Task<IActionResult> GetOrdersByAreaId(int areaId)
        {
            try
            {
                var orders = await _odersService.GetOrderByAreaId(areaId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }



        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<ActionResult<OrdersDTOResponse>> Create(int customerId, [FromBody] OrdersDTORequest orderBody)
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
                var create = await _odersService.CreateOrderFromCartAsync(customerId, orderBody);
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
