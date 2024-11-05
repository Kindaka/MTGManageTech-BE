using MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleDetailController : ControllerBase
    {
        private readonly IScheduleDetailService _scheduleDetailService;
        private readonly IAuthorizeService _authorizeService;

        public ScheduleDetailController(IScheduleDetailService scheduleDetailService, IAuthorizeService authorizeService)
        {
            _scheduleDetailService = scheduleDetailService;
            _authorizeService = authorizeService;
        }
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPost("CreateScheduleDetailForStaff")]
        public async Task<IActionResult> CreateScheduleDetail([FromBody] List<ScheduleDetailDtoRequest> requests, int accountId)
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


            var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid("Bạn không có quyền.");
            }
            

            // Gọi dịch vụ để tạo danh sách lịch trình và nhận danh sách kết quả
            var results = await _scheduleDetailService.CreateScheduleDetail(requests, accountId);

            // Kiểm tra nếu có lỗi trong kết quả
            if (results.Any(r => !r.Contains("thành công")))
            {
                return BadRequest(new { message = "Một số lịch trình không thể tạo.", details = results });
            }

            return Ok(new { message = "Tất cả lịch trình đã được tạo thành công.", details = results });
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("UpdateScheduleDetailForStaff/{Id}")]
        public async Task<IActionResult> UpdateScheduleDetail(int Id, int slotId, DateTime Date, int accountId)
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


            var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid("Bạn không có quyền.");
            }


            // Gọi dịch vụ để tạo danh sách lịch trình và nhận danh sách kết quả
            var result = await _scheduleDetailService.UpdateScheduleDetail(slotId, Date , accountId, Id);

            // Kiểm tra nếu có lỗi trong kết quả
            if (!result.Contains("thành công"))
            {
                return BadRequest(new { messaege = result });
            }

            return Ok(new { message = result });
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("GetScheduleDetailForStaff")]
        public async Task<IActionResult> GetScheduleDetailForStaff(int accountId, int slotId, DateTime Date)
        {
            try
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


                var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, accountId);
                if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
                {
                    return Forbid("Bạn không có quyền.");
                }
                var scheduleList = await _scheduleDetailService.GetScheduleDetailStaff(accountId, slotId, Date);
                return Ok(scheduleList);
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("GetSchedulesForStaffFiltterDate")]
        public async Task<IActionResult> GetScheduleForStaffWithDate(int accountId, [Required] DateTime FromDate, [Required] DateTime ToDate)
        {
            try
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


                var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, accountId);
                if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
                {
                    return Forbid("Bạn không có quyền.");
                }
                var scheduleList = await _scheduleDetailService.GetSchedulesStaff(accountId, FromDate, ToDate);
                return Ok(scheduleList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
