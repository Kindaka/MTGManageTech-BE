using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public DashboardController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats([FromQuery] int year)
        {
            if (year <= 0)
                return BadRequest("Invalid year");

            var stats = await _statisticService.GetDashboard(year);
            return Ok(stats);
        }
        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpGet("getStatsByArea")]
        public async Task<IActionResult> GetDashboardStatsByArea([FromQuery] int year, int areaId)
        {
            if (year <= 0)
                return BadRequest("Invalid year");

            var stats = await _statisticService.GetDashboardByAreaId(year, areaId);
            return Ok(stats);
        }

        [HttpGet("performance")]
        public async Task<IActionResult> GetStaffPerformance([FromQuery] int staffId, [FromQuery] int month, [FromQuery] int year)
        {
            if (staffId <= 0 || month <= 0 || month > 12 || year <= 0)
                return BadRequest("Invalid parameters");

            var performance = await _statisticService.GetWorkPerformanceStaff(staffId, month, year);
            return Ok(performance);
        }
    }
}
