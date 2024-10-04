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

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOdersService _odersService;

        public OrdersController(IOdersService odersService)
        {
            _odersService = odersService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersDTOResponse>>> GetOrders()
        {
            var orders = await _odersService.GetAll();
            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersDTOResponse>> GetOrder(int id)
        {
            var orders = await _odersService.GetById(id);

            if (orders == null)
            {
                return NotFound();
            }

            return Ok(orders);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrdersDTORequest ordersDTO)
        {
            try
            {
                var update = await _odersService.UpdateAsync(id, ordersDTO);


                if (update == null)
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrdersDTOResponse>> Create(OrdersDTORequest ordersDTO)
        {
            try
            {
                var create = await _odersService.CreateAsync(ordersDTO);
                return CreatedAtAction(nameof(GetOrder), new { id = create.OrderId }, create);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
