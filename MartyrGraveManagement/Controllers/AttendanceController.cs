using MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IAuthorizeService _authorizeService;
        public AttendanceController(IAttendanceService attendanceService, IAuthorizeService authorizeService)
        {
            _attendanceService = attendanceService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("CheckAttendance")]
        public async Task<IActionResult> CheckAttendanceStaff([FromBody] List<CheckAttendancesDtoRequest> checkList, int managerId)
        {
            if (checkList.Count() == 0)
            {
                return NotFound("Danh sách điểm danh bị trống");
            }
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != managerId)
            {
                return Forbid("Bạn không có quyền.");
            }

            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }

            try
            {
                // Gọi service để lấy danh sách lịch trình
                var attendances = await _attendanceService.CheckAttendance(checkList);

                // Kiểm tra nếu có lỗi trong kết quả
                if (attendances.Any(r => !r.Contains("thành công")))
                {
                    return BadRequest(new { message = attendances });
                }

                return Ok(new { message = attendances });


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

        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("GetAttendances")]
        public async Task<IActionResult> GetAttendanceStaff(int managerId)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != managerId)
            {
                return Forbid("Bạn không có quyền.");
            }

            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }

            try
            {
                // Gọi service để lấy danh sách lịch trình
                var attendances = await _attendanceService.GetAttendances(managerId);
                return Ok(attendances);
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

        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("GetAttendancesByScheduleId/{scheduleId}")]
        public async Task<IActionResult> GetAttendanceByScheduleId(int scheduleId, int managerId)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != managerId)
            {
                return Forbid("Bạn không có quyền.");
            }

            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }

            try
            {
                // Gọi service để lấy danh sách lịch trình
                var attendances = await _attendanceService.GetAttendancesByScheduleId(scheduleId);
                return Ok(attendances);
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

    }
}
