using MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MartyrGraveManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffPerformanceController : ControllerBase
    {
        private readonly IStaffPerformanceService _performanceService;
        private readonly ILogger<StaffPerformanceController> _logger;

        public StaffPerformanceController(
            IStaffPerformanceService performanceService,
            ILogger<StaffPerformanceController> logger)
        {
            _performanceService = performanceService;
            _logger = logger;
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost("evaluate")]
        public async Task<IActionResult> EvaluateStaffPerformance(
            [FromBody] StaffPerformanceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation(
                    "Starting performance evaluation. StaffId: {StaffId}, StartDate: {StartDate}, EndDate: {EndDate}", 
                    request.StaffId, 
                    request.StartDate.ToString("yyyy-MM-dd"), 
                    request.EndDate.ToString("yyyy-MM-dd")
                );

                var performance = await _performanceService.EvaluatePerformance(
                    request.StaffId,
                    request.StartDate,
                    request.EndDate);

                if (performance == null)
                {
                    _logger.LogWarning("No performance data returned for StaffId: {StaffId}", request.StaffId);
                    return NotFound($"No performance data found for staff {request.StaffId}");
                }

                _logger.LogInformation(
                    "Performance evaluation completed. StaffId: {StaffId}, QualityPoint: {QualityPoint}, TimePoint: {TimePoint}, InteractionPoint: {InteractionPoint}",
                    request.StaffId,
                    performance.QualityMaintenancePoint,
                    performance.TimeCompletePoint,
                    performance.InteractionPoint
                );

                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating staff performance for StaffId: {StaffId}", request.StaffId);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("staff/{staffId}")]
        public async Task<IActionResult> GetStaffPerformanceHistory(
            int staffId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Validate dates if both are provided
                if ((startDate.HasValue && !endDate.HasValue) || (!startDate.HasValue && endDate.HasValue))
                {
                    return BadRequest("Phải cung cấp cả startDate và endDate hoặc không cung cấp cả hai");
                }

                if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                {
                    return BadRequest("startDate phải nhỏ hơn hoặc bằng endDate");
                }

                _logger.LogInformation(
                    "Retrieving performance history for StaffId: {StaffId}, StartDate: {StartDate}, EndDate: {EndDate}, Page: {Page}, PageSize: {PageSize}",
                    staffId,
                    startDate?.ToString("yyyy-MM-dd") ?? "all",
                    endDate?.ToString("yyyy-MM-dd") ?? "all",
                    page,
                    pageSize
                );

                var result = await _performanceService.GetStaffPerformanceHistory(
                    staffId,
                    startDate,
                    endDate,
                    page,
                    pageSize
                );

                if (!result.performances.Any())
                {
                    _logger.LogWarning("No performance history found for StaffId: {StaffId}", staffId);
                    return NotFound($"Không tìm thấy lịch sử đánh giá cho nhân viên {staffId}");
                }

                return Ok(new { performanceList = result.performances, totalPage = result.totalPage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving performance history for StaffId: {StaffId}", staffId);
                return StatusCode(500, new { Message = ex.Message });
            }
        }



        /// <summary>
        /// Download excel file (Manager role)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("report/{workId}")]
        public async Task<IActionResult> DownloadPerformanceReportById(int workId)
        {
            try
            {
                _logger.LogInformation(
                    "Generating performance report for WorkId: {WorkId}",
                    workId
                );

                var reportStream = await _performanceService.GenerateStaffPerformanceReport(workId);

                // Lấy thông tin đánh giá để đặt tên file
                var performance = await _performanceService.GetPerformanceById(workId);
                string fileName = $"Performance_Report_{performance.AccountId}_{performance.StartDate:yyyyMMdd}_{performance.EndDate:yyyyMMdd}.xlsx";

                return File(
                    reportStream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating performance report for WorkId: {WorkId}", workId);
                return StatusCode(500, new { Message = "Lỗi khi tạo báo cáo: " + ex.Message });
            }
        }

        /// <summary>
        /// Download excel file (Staff role)
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("my-report/{workId}")]
        public async Task<IActionResult> DownloadMyPerformanceReportById(int workId)
        {
            try
            {
                // Lấy ID của nhân viên đang đăng nhập
                var staffId = int.Parse(User.FindFirst("AccountId")?.Value ?? "0");
                if (staffId == 0)
                {
                    return Unauthorized("Không thể xác định nhân viên");
                }

                // Kiểm tra xem đánh giá có phải của nhân viên này không
                var performance = await _performanceService.GetPerformanceById(workId);
                if (performance.AccountId != staffId)
                {
                    return Forbid("Bạn không có quyền xem đánh giá này");
                }

                _logger.LogInformation(
                    "Staff {StaffId} downloading their performance report for WorkId: {WorkId}",
                    staffId,
                    workId
                );

                var reportStream = await _performanceService.GenerateStaffPerformanceReport(workId);

                string fileName = $"My_Performance_Report_{performance.StartDate:yyyyMMdd}_{performance.EndDate:yyyyMMdd}.xlsx";

                return File(
                    reportStream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating performance report for WorkId: {WorkId}", workId);
                return StatusCode(500, new { Message = "Lỗi khi tạo báo cáo: " + ex.Message });
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("update/{workId}")]
        public async Task<ActionResult<WorkPerformanceDTO>> UpdatePerformance(
        int workId,
        [FromBody] UpdatePerformanceDTO updateDTO)
        {
            try
            {
                var result = await _performanceService.UpdatePerformance(workId, updateDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Policy = "RequireManagerRole")]
        [HttpDelete("{workId}")]
        public async Task<IActionResult> DeletePerformance(int workId)
        {
            try
            {
                var result = await _performanceService.DeletePerformance(workId);
                if (result)
                {
                    return Ok(new { Message = "Xóa đánh giá thành công" });
                }
                return BadRequest("Không thể xóa đánh giá");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("{workId}")]
public async Task<IActionResult> GetPerformanceById(int workId)
{
    try
    {
        _logger.LogInformation("Retrieving performance details for WorkId: {WorkId}", workId);

        var performance = await _performanceService.GetPerformanceById(workId);
        
        if (performance == null)
        {
            _logger.LogWarning("Performance not found for WorkId: {WorkId}", workId);
            return NotFound($"Không tìm thấy đánh giá với ID {workId}");
        }

        return Ok(performance);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving performance details for WorkId: {WorkId}", workId);
        return StatusCode(500, new { Message = ex.Message });
    }
}
    }

   
}
