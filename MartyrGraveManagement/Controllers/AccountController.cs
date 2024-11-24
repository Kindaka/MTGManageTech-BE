using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAuthorizeService _authorizeService;
        public AccountController(IAccountService accountService, IAuthorizeService authorizeService)
        {
            _accountService = accountService;
            _authorizeService = authorizeService;
        }
        /// <summary>
        /// Get all Staff Account By Area (Manager Role)
        /// </summary>
        /// <returns>Returns a list of all Staff Account.</returns>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("/api/staffs")]
        public async Task<ActionResult<IEnumerable<AccountDtoResponse>>> GetStaffs([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? areaId = null)
        {
            try
            {
                var staffs = await _accountService.GetStaffList(page, pageSize, areaId);
                return Ok(new { staffList = staffs.staffList, totalPage = staffs.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all Manager Account (Admin role)
        /// </summary>
        /// <returns>Returns a list of all Manager Account.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("/api/managers")]
        public async Task<ActionResult<IEnumerable<AccountDtoResponse>>> GetManagers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var managers = await _accountService.GetManagerList(page, pageSize);
                return Ok(new { managerList = managers.managerList, totalPage = managers.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Update account status (Admin and Manager Role)
        /// </summary>
        /// <returns>Returns a list of all Manager Account.</returns>
        [Authorize(Policy = "RequireManagerOrStaffOrAdminRole")]
        [HttpPut("/api/updateStatus/{banAccountId}")]
        public async Task<IActionResult> UpdateStatus(int banAccountId, int userAccountId)
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

                // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
                if (tokenAccountId != userAccountId)
                {
                    return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
                }

                // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
                var checkAuthorize = await _authorizeService.CheckAuthorizeStaffOrManager(tokenAccountId, userAccountId);
                if (!checkAuthorize.isMatchedStaffOrManager || !checkAuthorize.isAuthorized)
                {
                    return Forbid();
                }
                var check = await _accountService.ChangeStatusUser(banAccountId, userAccountId);
                if (check)
                {
                    return Ok("Thay đổi trạng thái user thành công");
                }
                else
                {
                    return BadRequest("Không thể cập nhật trạng thái");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update profile for Staff or Manager (Role 2 and 3)
        /// </summary>
        /// <returns>Returns success or failure status.</returns>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpPut("update-profile-staff-or-manager/{accountId}")]
        public async Task<IActionResult> UpdateProfileForStaffOrManager(int accountId, [FromBody] UpdateProfileStaffOrManagerDtoRequest updateProfileDto)
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

                // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
                if (tokenAccountId != accountId)
                {
                    return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
                }

                // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
                var checkAuthorize = await _authorizeService.CheckAuthorizeStaffOrManager(tokenAccountId, accountId);
                if (!checkAuthorize.isMatchedStaffOrManager || !checkAuthorize.isAuthorized)
                {
                    return Forbid();
                }

                // Cập nhật thông tin tài khoản
                var result = await _accountService.UpdateProfileForStaffOrManager(accountId, updateProfileDto);
                if (result)
                {
                    return Ok("Cập nhật thông tin thành công.");
                }
                else
                {
                    return BadRequest("Cập nhật thông tin thất bại.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("getProfile/{accountId}")]
        public async Task<IActionResult> GetAccountProfile(int accountId)
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

                // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
                if (tokenAccountId != accountId)
                {
                    return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
                }

                // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
                var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(tokenAccountId, accountId);
                if (!checkAuthorize.isMatchedAccount || !checkAuthorize.isAuthorizedAccount)
                {
                    return Forbid();
                }

                // Cập nhật thông tin tài khoản
                var result = await _accountService.GetAccountProfile(accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }






    }
}
