using MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {

        private readonly IScheduleService _scheduleService;
        private readonly IAuthorizeService _authorizeService;

        public ScheduleController(IScheduleService scheduleService, IAuthorizeService authorizeService)
        {
            _scheduleService = scheduleService;
            _authorizeService = authorizeService;
        }


        [HttpPost("CreateSchedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleDTORequest request)
        {
            var result = await _scheduleService.CreateSchedule(request);
            if (result == "Lịch trình được tạo thành công.")
            {
                return Ok(new { message = result });
            }
            else
            {
                return BadRequest(new { message = result });
            }
        }
    }
}
