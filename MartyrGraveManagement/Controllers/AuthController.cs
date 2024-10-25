using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticate users and generate tokens..
        /// </summary>
        /// <param name="loginInfo">The user's login information (email and password).</param>
        /// <returns>
        /// Returns an access token if the user is successfully authenticated, otherwise returns an error message.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] UserAuthenticatingDtoRequest loginInfo)
        {
            try
            {
                IActionResult response = Unauthorized();
                var isAuthenticated = await _authService.AuthenticateUser(loginInfo);
                if (isAuthenticated != null)
                {
                    if (isAuthenticated.Status == false)
                    {
                        return BadRequest("Tài khoản của bạn đã bị khóa");
                    }
                    var accessToken = await _authService.GenerateAccessToken(isAuthenticated);
                    if (accessToken.IsNullOrEmpty())
                    {
                        return BadRequest("Something went wrong");
                    }
                    response = Ok(new { accessToken = accessToken });
                    return response;
                }
                return NotFound("Sai số điện thoại hoặc mật khẩu");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        /// <summary>
        /// Register account for Manager / Staff / Admin
        /// </summary>
        /// <param name="newAccount">The new account information including email, password, and confirmation password.</param>
        /// <returns>
        /// Returns a success message if the account is successfully created, otherwise returns an error message.
        /// </returns>
        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPost("register-account-martyrGrave")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDtoRequest newAccount)
        {
            try
            {
                if(newAccount.RoleId == 4)
                {
                    return BadRequest("Role của bạn không có quyền");
                }
                if (!newAccount.Password.Equals(newAccount.ConfirmPassword))
                {
                    return BadRequest("Not matching password");
                }
                if (!(await _authService.GetAccountByPhoneNumber(newAccount.PhoneNumber)).status)
                {
                    bool checkRegister = await _authService.CreateAccount(newAccount);
                    if (checkRegister)
                    {
                        return Ok("Create success");
                    }
                    else
                    {
                        return BadRequest("Cannot create account");
                    }
                }
                else
                {
                    return BadRequest("Existed phonenumber");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(500, $"Internal Server Error: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

        }

        [AllowAnonymous]
        [HttpPost("register-account-guest")]
        public async Task<IActionResult> RegisterGuestWithPhone([FromBody] CustomerRegisterDtoRequest newCustomer)
        {
            try
            {
                if (newCustomer.PhoneNumber.Length != 10)
                {
                    return BadRequest("Số điện thoại phải có 10 kí tự.");
                }
                if (!newCustomer.Password.Equals(newCustomer.ConfirmPassword))
                {
                    return BadRequest("Mật khẩu không khớp");
                }
                var isGuest = await _authService.GetAccountByPhoneNumber(newCustomer.PhoneNumber);
                if (!isGuest.status)
                {
                    bool checkRegister = await _authService.CreateAccountCustomer(newCustomer);
                    if (checkRegister)
                    {
                        return Ok("Tạo tài khoản thành công");
                    }
                    else
                    {
                        return BadRequest("Tạo tài khoản thất bại");
                    }
                }
                else
                {
                    return BadRequest("Số điện thoại đã tồn tại");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(500, $"Internal Server Error: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

        }

        //[AllowAnonymous]
        //[HttpGet("auth-guest/{phone}")]
        //public async Task<IActionResult> LoginForGuest(string phone)
        //{
        //    try
        //    {
        //        IActionResult response = Unauthorized();
        //        var isGuest = await _authService.GetAccountByPhoneNumber(phone);
        //        if (isGuest.status && isGuest.guest != null)
        //        {
        //            if (isGuest.guest.Status == false)
        //            {
        //                return BadRequest("Your account is locked by administrator");
        //            }
        //            var accessToken = await _authService.GenerateAccessToken(isGuest.guest);
        //            if (accessToken.IsNullOrEmpty())
        //            {
        //                return BadRequest("Something went wrong");
        //            }
        //            response = Ok(new { accessToken = accessToken });
        //            return response;
        //        }
        //        return NotFound("Sai SĐT hoặc bàn không có quyền truy cập");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal Server Error: {ex.Message}");
        //    }
        //}
    }
}
