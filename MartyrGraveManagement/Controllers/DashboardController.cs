using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("performance")]
        public async Task<IActionResult> GetStaffPerformance([FromQuery] int staffId, [FromQuery] int month, [FromQuery] int year)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            if (staffId <= 0 || month <= 0 || month > 12 || year <= 0)
                return BadRequest("Invalid parameters");

            var performance = await _statisticService.GetWorkPerformanceStaff(staffId, tokenAccountId, month, year);
            return Ok(performance);
        }

        [HttpGet("export-work-performance-csv")]
        public async Task<IActionResult> ExportWorkPerformanceToCsv(int staffId, int month, int year)
        {
            try
            {
                // Lấy AccountId từ token
                var tokenAccountIdClaim = User.FindFirst("AccountId");
                if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
                {
                    return Forbid("Không tìm thấy AccountId trong token.");
                }

                var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);
                // Lấy dữ liệu từ hàm GetWorkPerformanceStaff
                var workPerformance = await _statisticService.GetWorkPerformanceStaff(staffId, tokenAccountId, month, year);

                // Kiểm tra dữ liệu
                if (workPerformance == null)
                {
                    return NotFound("Không tìm thấy dữ liệu hiệu suất làm việc.");
                }

                // Tạo danh sách dữ liệu cho CSV
                var csvData = new List<object>
        {
            new
            {
                TổngCôngViệcThường = workPerformance.totalTask,
                TổngCôngViệcĐịnhKì = workPerformance.totalAssignmentTask,
                TổngCôngViệcYêuCầu = workPerformance.totalRequestTask,
                TổngCôngViệcThườngHoànThành = workPerformance.totalFinishTask,
                TổngĐịnhKìHoànThành = workPerformance.totalFinishAssignmentTask,
                TổngYêuCầuHoànThành = workPerformance.totalFinishRequestTask,
                TổngCôngViệcThấtBại = workPerformance.totalFailTask,
                TổngĐịnhKìThấtBại = workPerformance.totalFailAssignmentTask,
                TổngYêuCầuThấtBại = workPerformance.totalFailRequestTask,
                HiệuSuất = workPerformance.workPerformance,
                ChấtLượngCôngViệc = workPerformance.workQuality,
                ĐánhGiáTrungBình = workPerformance.averageAllFeedbackRate
            }
        };

                // Tạo file CSV với mã hóa UTF-8
                var memoryStream = new MemoryStream();
                using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true), leaveOpen: true)) // Sử dụng leaveOpen: true
                using (var csvWriter = new CsvHelper.CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteRecords(csvData);
                }
                memoryStream.Position = 0;

                var fileName = $"WorkPerformance_Staff_{staffId}_{month}_{year}.csv";
                return File(memoryStream, "text/csv; charset=utf-8", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi xuất file CSV: {ex.Message}");
            }
        }

    }
}
