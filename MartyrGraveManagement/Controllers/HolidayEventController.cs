using MartyrGraveManagement_BAL.ModelViews.HolidayEventDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayEventController : ControllerBase
    {
        private readonly IHolidayEventService _holidayEventService;
        private readonly IAuthorizeService _authorizeService;
        public HolidayEventController(IHolidayEventService holidayEventService, IAuthorizeService authorizeService)
        {
            _holidayEventService = holidayEventService;
            _authorizeService = authorizeService;
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public async Task<IActionResult> GetAllHolidayEvents()
        {
            var events = await _holidayEventService.GetAllHolidayEventsAsync();
            return Ok(events);
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHolidayEventById(int id)
        {
            var holidayEvent = await _holidayEventService.GetHolidayEventByIdAsync(id);
            if (holidayEvent == null)
            {
                return NotFound("Holiday event not found.");
            }
            return Ok(holidayEvent);
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<IActionResult> CreateHolidayEvent([FromBody] HolidayEventRequestDto holidayEventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy AccountId từ token
            var accountIdClaim = User.FindFirst("AccountId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return Forbid("Token does not contain AccountId.");
            }

            // Chuyển AccountId từ string sang int
            int accountId = int.Parse(accountIdClaim);

            // Gọi service để tạo sự kiện với accountId và DTO
            var result = await _holidayEventService.CreateHolidayEventAsync(accountId, holidayEventDto);
            if (!result)
            {
                return StatusCode(500, "An error occurred while creating the holiday event.");
            }
            return Ok("Holiday event created successfully.");
        }




        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHolidayEvent(int id, [FromBody] HolidayEventRequestDto holidayEventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy AccountId từ token
            var accountIdClaim = User.FindFirst("AccountId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return Forbid("Token does not contain AccountId.");
            }

            // Chuyển AccountId từ string sang int và gán vào DTO
            int accountId = int.Parse(accountIdClaim);

            // Gọi service để cập nhật sự kiện với accountId và DTO
            var result = await _holidayEventService.UpdateHolidayEventAsync(id, accountId, holidayEventDto);
            if (!result)
            {
                return NotFound("Holiday event not found.");
            }

            return Ok("Update Successfully");
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHolidayEvent(int id)
        {
            var result = await _holidayEventService.DeleteHolidayEventAsync(id);
            if (!result)
            {
                return NotFound("Holiday event not found.");
            }

            return Ok("Delete Successfully");
        }



        [Authorize(Policy = "RequireAdminRole")]
        [HttpPatch("{id}/status/{status}")]
        public async Task<IActionResult> UpdateHolidayEventStatus(int id, bool status)
        {
            var result = await _holidayEventService.UpdateHolidayEventStatusAsync(id, status);
            if (!result)
            {
                return NotFound("Holiday event not found.");
            }

            return Ok("Status updated successfully.");
        }

    }
}
