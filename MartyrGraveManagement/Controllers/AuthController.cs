using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
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
                        return BadRequest("Your account is locked by administrator");
                    }
                    var accessToken = await _authService.GenerateAccessToken(isAuthenticated);
                    if (accessToken.IsNullOrEmpty())
                    {
                        return BadRequest("Something went wrong");
                    }
                    response = Ok(new { accessToken = accessToken });
                    return response;
                }
                return NotFound("Wrong email or password");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        /// <summary>
        /// Register customer account(Role Customer is 4 and in the DB must INSERT data first).
        /// </summary>
        /// <param name="newAccount">The new account information including email, password, and confirmation password.</param>
        /// <returns>
        /// Returns a success message if the account is successfully created, otherwise returns an error message.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("register-customer")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDtoRequest newAccount)
        {
            try
            {
                if (!newAccount.Password.Equals(newAccount.ConfirmPassword))
                {
                    return BadRequest("Not matching password");
                }
                if (!await _authService.GetAccountByEmail(newAccount.EmailAddress))
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
                    return BadRequest("Existed email");
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
    }
}
