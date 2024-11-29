using MartyrGraveManagement_BAL.ModelViews.HistoricalEventDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoricalEventController : Controller
    {
        private readonly IHistoricalEventService _historicalEventService;
        private readonly IAuthorizeService _authorizeService;


        public HistoricalEventController(IHistoricalEventService historicalEventService, IAuthorizeService authorizeService)
        {
            _historicalEventService = historicalEventService;
            _authorizeService = authorizeService;
        }

        [HttpGet("GetAllHistoricalEvents")]
        public async Task<IActionResult> GetAllHistoricalEvents()
        {
            try
            {
                var historicalEvents = await _historicalEventService.GetAllHistoricalEvents();

                if (historicalEvents == null || historicalEvents.Count == 0)
                {
                    return NotFound(new { message = "No historical events found." });
                }

                return Ok(historicalEvents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving data.", error = ex.Message });
            }
        }

        [HttpGet("GetHistoricalEventsByAccount")]
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        public async Task<IActionResult> GetHistoricalEventsByAccount()
        {
            try
            {
                // Lấy AccountId từ token JWT của người dùng hiện tại
                var userAccountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Kiểm tra quyền truy cập (giới hạn theo khu vực)
                var (isMatchedStaffOrManager, isAuthorized) = await _authorizeService.CheckAuthorizeStaffOrManager(userAccountId, userAccountId);
                if (!isAuthorized)
                {
                    return Forbid();
                }

                // Gọi phương thức dịch vụ để lấy các sự kiện lịch sử theo quyền hạn của tài khoản
                var historicalEvents = await _historicalEventService.GetHistoricalEventByAccount(userAccountId);

                if (historicalEvents == null || historicalEvents.Count == 0)
                {
                    return NotFound(new { message = "No historical events found." });
                }

                return Ok(historicalEvents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving data.", error = ex.Message });
            }
        }

        [HttpGet("GetHistoricalEventById/{historicalEventId}")]
        public async Task<IActionResult> GetHistoricalEventById(int historicalEventId)
        {
            try
            {
                var historicalEvent = await _historicalEventService.GetHistoricalEventById(historicalEventId);

                if (historicalEvent == null)
                {
                    return NotFound(new { message = "Historical event not found." });
                }

                return Ok(historicalEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the historical event.", error = ex.Message });
            }
        }

        [HttpPost("CreateHistoricalEvent")]
        [Authorize(Policy = "RequireManagerRole")] 
        public async Task<IActionResult> CreateHistoricalEvent([FromBody] CreateHistoricalEventDTORequest newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest(new { message = "Invalid event data." });
            }

            try
            {
                var result = await _historicalEventService.CreateHistoricalEvent(newEvent);

                if (result == null)
                {
                    return BadRequest(new { message = "Failed to create historical event." });
                }

                return Ok(new { message = "Historical event created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the historical event.", error = ex.Message });
            }
        }

        [HttpPut("UpdateHistoricalEvent/{historyId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> UpdateHistoricalEvent(int historyId, [FromBody] CreateHistoricalEventDTORequest updateRequest)
        {
            try
            {
                var updatedEvent = await _historicalEventService.UpdateHistoricalEvent(historyId, updateRequest);
                return Ok(updatedEvent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Historical event not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the historical event.", error = ex.Message });
            }
        }

        [HttpPatch("UpdateHistoricalEventStatus/{historyId}/{newStatus}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateHistoricalEventStatus(int historyId, bool newStatus)
        {
            try
            {
                var updatedEvent = await _historicalEventService.UpdateHistoricalEventStatus(historyId, newStatus);
                return Ok(updatedEvent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Historical event not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the historical event status.", error = ex.Message });
            }
        }


    }
}
