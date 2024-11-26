//using MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs;
//using MartyrGraveManagement_BAL.Services.Implements;
//using MartyrGraveManagement_BAL.Services.Interfaces;
//using MartyrGraveManagement_DAL.Entities;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;

//namespace MartyrGraveManagement.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AttendanceController : ControllerBase
//    {
//        private readonly IAttendanceService _attendanceService;
//        private readonly IAuthorizeService _authorizeService;
//        public AttendanceController(IAttendanceService attendanceService, IAuthorizeService authorizeService)
//        {
//            _attendanceService = attendanceService;
//            _authorizeService = authorizeService;
//        }

//        [Authorize(Policy = "RequireManagerRole")]
//        [HttpPut("CheckAttendancesForManager")]
//        public async Task<IActionResult> CheckAttendancesManager([FromBody] List<CheckAttendancesDtoRequest> checkList, int managerId)
//        {
//            if (checkList.Count() == 0)
//            {
//                return NotFound("Danh sách điểm danh bị trống");
//            }
//            // Lấy AccountId từ token
//            var tokenAccountIdClaim = User.FindFirst("AccountId");
//            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//            {
//                return Forbid("Không tìm thấy AccountId trong token.");
//            }

//            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//            if (tokenAccountId != managerId)
//            {
//                return Forbid("Bạn không có quyền.");
//            }

//            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
//            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
//            {
//                return Forbid();
//            }

//            try
//            {
//                // Gọi service để lấy danh sách lịch trình
//                var attendances = await _attendanceService.CheckAttendances(checkList, managerId);

//                // Kiểm tra nếu có lỗi trong kết quả
//                if (attendances.Any(r => !r.Contains("thành công")))
//                {
//                    return BadRequest(new { message = attendances });
//                }

//                return Ok(new { message = attendances });


//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return StatusCode(403, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//            }
//        }

//        [Authorize(Policy = "RequireStaffRole")]
//        [HttpPut("CheckAttendanceForStaff")]
//        public async Task<IActionResult> CheckAttendanceForStaff([FromBody] CheckAttendanceForStaffDtoRequest request, int staffId)
//        {
//            // Lấy AccountId từ token
//            var tokenAccountIdClaim = User.FindFirst("AccountId");
//            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//            {
//                return Forbid("Không tìm thấy AccountId trong token.");
//            }

//            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//            if (tokenAccountId != staffId)
//            {
//                return Forbid("Bạn không có quyền.");
//            }

//            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//            var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, staffId);
//            if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
//            {
//                return Forbid();
//            }

//            try
//            {
//                // Gọi service để lấy danh sách lịch trình
//                var attendance = await _attendanceService.CheckAttendanceForStaff(request, staffId);

//                // Kiểm tra nếu có lỗi trong kết quả
//                if (attendance.status == false)
//                {
//                    return BadRequest(new { message = attendance.responseContent });
//                }

//                return Ok(new { message = attendance.responseContent });


//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return StatusCode(403, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//            }
//        }

//        [Authorize(Policy = "RequireManagerRole")]
//        [HttpPut("UpdateSingleAttendanceStatus")]
//        public async Task<IActionResult> UpdateAttendanceStatus(int attendanceId, int status, string? Note, int managerId)
//        {
//            // Lấy AccountId từ token
//            var tokenAccountIdClaim = User.FindFirst("AccountId");
//            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//            {
//                return Forbid("Không tìm thấy AccountId trong token.");
//            }

//            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//            if (tokenAccountId != managerId)
//            {
//                return Forbid("Bạn không có quyền.");
//            }

//            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
//            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
//            {
//                return Forbid();
//            }

//            try
//            {
//                // Gọi service để lấy danh sách lịch trình
//                var attendance = await _attendanceService.UpdateAttendancStatus(attendanceId, status, Note, managerId);

//                // Kiểm tra nếu có lỗi trong kết quả
//                if (!attendance.Contains("thành công"))
//                {
//                    return BadRequest(new { message = attendance });
//                }

//                return Ok(new { message = attendance });


//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return StatusCode(403, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//            }
//        }

//        //[Authorize(Policy = "RequireManagerRole")]
//        //[HttpGet("GetAttendances/{managerId}")]
//        //public async Task<IActionResult> GetAttendancesStaff(int managerId)
//        //{
//        //    // Lấy AccountId từ token
//        //    var tokenAccountIdClaim = User.FindFirst("AccountId");
//        //    if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//        //    {
//        //        return Forbid("Không tìm thấy AccountId trong token.");
//        //    }

//        //    var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//        //    // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//        //    if (tokenAccountId != managerId)
//        //    {
//        //        return Forbid("Bạn không có quyền.");
//        //    }

//        //    // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//        //    var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
//        //    if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
//        //    {
//        //        return Forbid();
//        //    }

//        //    try
//        //    {
//        //        // Gọi service để lấy danh sách lịch trình
//        //        var attendances = await _attendanceService.GetAttendances(managerId);
//        //        return Ok(attendances);
//        //    }
//        //    catch (KeyNotFoundException ex)
//        //    {
//        //        return NotFound(new { message = ex.Message });
//        //    }
//        //    catch (UnauthorizedAccessException ex)
//        //    {
//        //        return StatusCode(403, new { message = ex.Message });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//        //    }
//        //}

//        [Authorize(Policy = "RequireStaffRole")]
//        [HttpGet("GetAttendanceByStaffId/{staffId}")]
//        public async Task<IActionResult> GetAttendancesByStaffId(DateTime Date, int staffId)
//        {
//            // Lấy AccountId từ token
//            var tokenAccountIdClaim = User.FindFirst("AccountId");
//            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//            {
//                return Forbid("Không tìm thấy AccountId trong token.");
//            }

//            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//            if (tokenAccountId != staffId)
//            {
//                return Forbid("Bạn không có quyền.");
//            }

//            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//            var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, staffId);
//            if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
//            {
//                return Forbid();
//            }

//            try
//            {
//                // Gọi service để lấy danh sách lịch trình
//                var attendances = await _attendanceService.GetAttendancesByStaffId(Date, staffId);
//                return Ok(attendances);
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return StatusCode(403, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//            }
//        }

//        [Authorize(Policy = "RequireManagerOrStaffRole")]
//        [HttpGet("GetAttendanceByAttendanceId/{attendanceId}")]
//        public async Task<IActionResult> GetAttendanceByAttedanceId(int attendanceId)
//        {
//            // Lấy AccountId từ token
//            var tokenAccountIdClaim = User.FindFirst("AccountId");
//            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//            {
//                return Forbid("Không tìm thấy AccountId trong token.");
//            }


//            try
//            {
//                // Gọi service để lấy danh sách lịch trình
//                var attendances = await _attendanceService.GetAttendanceByAttendanceId(attendanceId);
//                return Ok(attendances);
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return StatusCode(403, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//            }
//        }

//        [Authorize(Policy = "RequireManagerRole")]
//        [HttpGet("GetAttendancesWithSlotAndDateForManager")]
//        public async Task<IActionResult> GetAttendanceWithSLotAndDateForManager(int slotId, DateTime Date, int managerId)
//        {
//            // Lấy AccountId từ token
//            var tokenAccountIdClaim = User.FindFirst("AccountId");
//            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//            {
//                return Forbid("Không tìm thấy AccountId trong token.");
//            }

//            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//            if (tokenAccountId != managerId)
//            {
//                return Forbid("Bạn không có quyền.");
//            }

//            // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//            var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
//            if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
//            {
//                return Forbid();
//            }

//            try
//            {
//                // Gọi service để lấy danh sách lịch trình
//                var attendances = await _attendanceService.GetAttendancesBySchedule(slotId, Date, managerId);
//                return Ok(attendances);
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return StatusCode(403, new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//            }
//        }

//        //[Authorize(Policy = "RequireStaffRole")]
//        //[HttpGet("GetAttendancesWithSlotAndDateForStaff")]
//        //public async Task<IActionResult> GetAttendanceWithSLotAndDateForStaff(int slotId, DateTime Date, int staffId)
//        //{
//        //    // Lấy AccountId từ token
//        //    var tokenAccountIdClaim = User.FindFirst("AccountId");
//        //    if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//        //    {
//        //        return Forbid("Không tìm thấy AccountId trong token.");
//        //    }

//        //    var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//        //    // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//        //    if (tokenAccountId != staffId)
//        //    {
//        //        return Forbid("Bạn không có quyền.");
//        //    }

//        //    // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//        //    var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, staffId);
//        //    if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
//        //    {
//        //        return Forbid();
//        //    }

//        //    try
//        //    {
//        //        // Gọi service để lấy danh sách lịch trình
//        //        var attendances = await _attendanceService.GetAttendancesByScheduleForStaff(slotId, Date, staffId);
//        //        return Ok(attendances);
//        //    }
//        //    catch (KeyNotFoundException ex)
//        //    {
//        //        return NotFound(new { message = ex.Message });
//        //    }
//        //    catch (UnauthorizedAccessException ex)
//        //    {
//        //        return StatusCode(403, new { message = ex.Message });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
//        //    }
//        //}


//        /// <summary>
//        /// Get slots and dates for attendances under a manager
//        /// </summary>
//        [Authorize(Policy = "RequireManagerRole")]
//        [HttpGet("get-list-slots-dates")]
//        public async Task<IActionResult> GetAttendanceSlotDates(
//            [FromQuery] DateTime startDate,
//            [FromQuery] DateTime endDate,
//            [FromQuery] int managerId)
//        {
//            try
//            {
//                if (startDate > endDate)
//                {
//                    return BadRequest(new
//                    {
//                        success = false,
//                        message = "Start date must be before or equal to end date"
//                    });
//                }
//                // Lấy AccountId từ token
//                var tokenAccountIdClaim = User.FindFirst("AccountId");
//                if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
//                {
//                    return Forbid("Không tìm thấy AccountId trong token.");
//                }

//                var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

//                // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
//                if (tokenAccountId != managerId)
//                {
//                    return Forbid("Bạn không có quyền.");
//                }

//                // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
//                var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
//                if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
//                {
//                    return Forbid();
//                }

//                var slotDates = await _attendanceService.GetAttendanceSlotDates(startDate, endDate, managerId);

//                if (!slotDates.Any())
//                {
//                    return Ok(new
//                    {
//                        success = true,
//                        message = "No attendance records found for the specified criteria",
//                        data = new List<AttendanceSlotDateDtoResponse>()
//                    });
//                }

//                return Ok(new
//                {
//                    success = true,
//                    message = "Attendance slots and dates retrieved successfully",
//                    data = slotDates
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    success = false,
//                    message = "Error retrieving attendance slots and dates",
//                    error = ex.Message
//                });
//            }
//        }
//    }
//}
