using MartyrGraveManagement_BAL.ModelViews.CommentReportDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentReportController : ControllerBase
    {
        private readonly ICommentReportService _commentReportService;

        public CommentReportController(ICommentReportService commentReportService)
        {
            _commentReportService = commentReportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommentReports()
        {
            var result = await _commentReportService.GetAllCommentReportsAsync();
            return Ok(result);
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetCommentReportById(int reportId)
        {
            try
            {
                var result = await _commentReportService.GetCommentReportByIdAsync(reportId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<IActionResult> CreateCommentReport([FromBody] CreateCommentReportDTO request)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var accountId = int.Parse(tokenAccountIdClaim.Value);

            try
            {
                var result = await _commentReportService.CreateCommentReportAsync(request.CommentId, request, accountId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tạo báo cáo bình luận: {ex.Message}" });
            }
        }

    }
}
