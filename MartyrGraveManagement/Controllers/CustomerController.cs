using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
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
        public CustomerController(ICustomerService customerService, IAuthService authService)
        {
            _customerService = customerService;
            _authService = authService;
        }

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
                if (await _authService.GetAccountByAccountName(account.AccountName))
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
    }
}
