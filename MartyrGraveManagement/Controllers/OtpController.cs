using MartyrGraveManagement_BAL.ModelViews.OtpDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequest request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                return BadRequest("Phone number is required");
            }

            bool result = await _otpService.SendOtpAsync(request.PhoneNumber);

            if (result)
            {
                return Ok(new { message = "OTP sent successfully" });
            }

            return BadRequest(new { message = "Failed to send OTP" });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest("Phone number and OTP are required");
            }

            bool result = await _otpService.VerifyOtpAsync(request.PhoneNumber, request.Otp);

            if (result)
            {
                return Ok(new { message = "OTP verified successfully" });
            }

            return BadRequest(new { message = "Invalid OTP" });
        }
    }
}
