using MartyrGraveManagement_BAL.ModelViews.GraveServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraveServiceController : ControllerBase
    {
        private readonly IGraveService_Service _serviceOfGrave;
        private readonly IAuthorizeService _authorizeService;
        public GraveServiceController(IGraveService_Service serviceOfGrave, IAuthorizeService authorizeService)
        {
            _serviceOfGrave = serviceOfGrave;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost]
        public async Task<IActionResult> CreateServiceGrave(int managerId, GraveServiceDtoRequest graveServiceDTO)
        {//
            try
            {
                if (graveServiceDTO == null)
                {
                    return BadRequest("Cannot add empty service to grave");
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
                    return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
                }

                var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
                if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
                {
                    return Forbid();
                }
                var check = await _serviceOfGrave.CreateServiceForGrave(graveServiceDTO);
                if (check.check)
                {
                    return Ok(check.response);
                }
                else
                {
                    return NotFound(check.response);
                }

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut]
        public async Task<IActionResult> UpdateServiceGrave(int martyrId, UpdateServiceForGraveDtoRequest graveServiceDTO)
        {//
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                if (graveServiceDTO == null)
                {
                    return BadRequest("Cannot add empty service to grave");
                }
                var check = await _serviceOfGrave.UpdateServiceForGrave(martyrId, graveServiceDTO);
                if (check.check)
                {
                    return Ok(check.response);
                }
                else
                {
                    return NotFound(check.response);
                }

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpDelete]
        public async Task<IActionResult> DeleteServiceGrave(int graveServiceId, int managerId)
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
                if (tokenAccountId != managerId)
                {
                    return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
                }

                var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
                if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
                {
                    return Forbid();
                }
                var check = await _serviceOfGrave.DeleteServiceOfGrave(graveServiceId);
                if (check.check)
                {
                    return Ok(check.response);
                }
                else
                {
                    return NotFound(check.response);
                }

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("grave-services")]
        public async Task<IActionResult> GetServicesInGrave(int martyrId, int categoryId)
        {
            try
            {
                var services = await _serviceOfGrave.GetAllServicesForGrave(martyrId, categoryId);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("service/notAvaiable/grave-services")]
        public async Task<IActionResult> GetServicesNotInGrave(int martyrId)
        {
            try
            {
                var services = await _serviceOfGrave.GetAllServicesNotInGrave(martyrId);
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
