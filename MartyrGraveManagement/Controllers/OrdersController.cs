﻿using System;
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

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _odersService;

        public OrdersController(IOrdersService odersService)
        {
            _odersService = odersService;
        }

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





        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<List<OrdersGetAllDTOResponse>>> GetOrderByAccountId(int accountId)
        {
            try
            {
                var orders = await _odersService.GetOrderByAccountId(accountId);

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


        [HttpPost]
        public async Task<ActionResult<OrdersDTOResponse>> Create(int accountId)
        {
            try
            {
                var create = await _odersService.CreateOrderFromCartAsync(accountId);
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




        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var deleted = await _odersService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok("Delete Successfully");
        }
    }
}