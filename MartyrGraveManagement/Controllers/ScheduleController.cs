using MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

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

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost("CreateSchedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] List<CreateScheduleDTORequest> requests, int accountId)
        {
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);
            if (tokenAccountId != accountId)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
            }

            foreach (var request in requests)
            {
                var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, accountId);
                if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
                var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, request.AccountId);
                if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
                {
                    return Forbid("Bạn không có quyền tạo lịch làm việc.");
                }
            }

            // Gọi dịch vụ để tạo danh sách lịch trình và nhận danh sách kết quả
            var results = await _scheduleService.CreateSchedule(requests, accountId);

            // Kiểm tra nếu có lỗi trong kết quả
            if (results.Any(r => !r.Contains("thành công")))
            {
                return BadRequest(new { message = "Một số lịch trình không thể tạo.", details = results });
            }

            return Ok(new { message = "Tất cả lịch trình đã được tạo thành công.", details = results });
        }





        /// <summary>
        /// Get List Schedule by AccountId (Staff Role)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("GetScheduleByAccountId/{accountId}")]
        public async Task<IActionResult> GetScheduleByAccountId(int accountId)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != accountId)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
            }

            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }

            try
            {
                // Gọi service để lấy danh sách lịch trình
                var schedules = await _scheduleService.GetScheduleByAccountId(accountId);

                // Trả về dữ liệu dưới dạng JSON
                return Ok(new { message = "Schedules retrieved successfully.", data = schedules });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


        /// <summary>
        /// Update Schedule.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule(int scheduleId, [FromBody] UpdateScheduleDTORequest request, int accountId)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != accountId)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
            }

            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }
            try
            {
                var result = await _scheduleService.UpdateSchedule(scheduleId, request);
                if (result == "Cập nhật Schedule thành công.")
                {
                    return Ok(new { message = result });
                }
                return BadRequest(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi cập nhật Schedule: {ex.Message}" });
            }
        }

        /// <summary>
        /// Delete Schedule.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(int scheduleId, int accountId)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != accountId)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
            }

            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }

            try
            {
                var result = await _scheduleService.DeleteSchedule(scheduleId);
                if (result == "Xóa Schedule thành công.")
                {
                    return Ok(new { message = result });
                }
                return BadRequest(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi xóa Schedule: {ex.Message}" });
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("GetScheduleById/{scheduleId}")]
        public async Task<IActionResult> GetScheduleById(int scheduleId, int accountId)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != accountId)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
            }

            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }
            try
            {
                var schedule = await _scheduleService.GetScheduleById(scheduleId);
                return Ok(new { message = "Schedule retrieved successfully.", data = schedule });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


    }
}
