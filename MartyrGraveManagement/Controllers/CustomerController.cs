using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IAuthService _authService;
        private readonly IAuthorizeService _authorizeService;

        public CustomerController(ICustomerService customerService, IAuthService authService, IAuthorizeService authorizeService)
        {
            _customerService = customerService;
            _authService = authService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("change-password-customer")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCustomerRequest account)
        {
            try
            {
                //var accountId = User.FindFirst("AccountId")?.Value;
                //if (accountId == null)
                //{
                //    return Forbid();
                //}
                //var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(account.customerId, int.Parse(accountId));
                //if (!checkMatchedId.isMatchedCustomer)
                //{
                //    return Forbid();
                //}
                if (!account.Password.Equals(account.ConfirmPassword))
                {
                    return BadRequest("Not matching password");
                }
                if ((await _authService.GetAccountByPhoneNumber(account.PhoneNumber)).status)
                {
                    var checkRegister = await _customerService.ChangePasswordCustomer(account);
                    if (checkRegister.status)
                    {
                        return Ok($"{checkRegister.responseContent}");
                    }
                    else
                    {
                        return BadRequest($"{checkRegister.responseContent}");
                    }
                }
                else
                {
                    return BadRequest("AccountName not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update Profile for Customer
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("update-profile/{accountId}")]
        public async Task<IActionResult> UpdateProfile(int accountId, [FromBody] UpdateProfileDtoRequest updateProfileDto)
        {
            try
            {
                // Lấy AccountId từ token
                var tokenAccountId = User.FindFirst("AccountId")?.Value;
                if (tokenAccountId == null)
                {
                    return Forbid();
                }

                // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
                if (int.Parse(tokenAccountId) != accountId)
                {
                    return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
                }

                // Kiểm tra quyền truy cập của khách hàng
                var authorizationResult = await _authorizeService.CheckAuthorizeByCustomerId(accountId, int.Parse(tokenAccountId));
                if (!authorizationResult.isMatchedCustomer)
                {
                    return Forbid();
                }

                // Cập nhật thông tin tài khoản
                var result = await _customerService.UpdateProfile(accountId, updateProfileDto);
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
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }


    }
}
