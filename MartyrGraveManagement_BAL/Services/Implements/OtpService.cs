using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly ISmsService _smsService;

        public OtpService(IMemoryCache cache, ISmsService smsService)
        {
            _cache = cache;
            _smsService = smsService;
        }

        public async Task<bool> SendOtpAsync(string phoneNumber)
        {
            try
            {
                // Format the phone number
                //string formattedNumber = FormatPhoneNumber(phoneNumber);
                // Generate 6-digit OTP
                string otp = GenerateOtp();

                // Store OTP in cache with 5 minutes expiration
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(phoneNumber, otp, cacheOptions);

                // Here you would typically send the OTP via SMS
                // For demonstration, we'll just print it to console
                Console.WriteLine($"OTP for {phoneNumber}: {otp}");

                // In production, you would use an SMS service like this:
                // await _smsService.SendSmsAsync(phoneNumber, $"Your OTP is: {otp}");

                return await _smsService.SendSmsAsync(phoneNumber, $"Your OTP is: {otp}"); ;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<bool> VerifyOtpAsync(string phoneNumber, string otp)
        {
            try
            {
                // Get OTP from cache
                if (_cache.TryGetValue(phoneNumber, out string storedOtp))
                {
                    // Verify OTP
                    bool isValid = storedOtp == otp;

                    if (isValid)
                    {
                        // Remove OTP from cache after successful verification
                        _cache.Remove(phoneNumber);
                    }

                    return isValid;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Remove all non-digit characters
            string digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // For Vietnam numbers
            if (digitsOnly.Length == 9) // Standard VN mobile number length
            {
                return $"+84{digitsOnly}";
            }
            else if (digitsOnly.StartsWith("84") && digitsOnly.Length == 11)
            {
                return $"+{digitsOnly}";
            }
            else if (digitsOnly.StartsWith("0")) // If number starts with 0
            {
                return $"+84{digitsOnly.Substring(1)}";
            }

            return phoneNumber; // Return original if no formatting rules match
        }
    }
}
