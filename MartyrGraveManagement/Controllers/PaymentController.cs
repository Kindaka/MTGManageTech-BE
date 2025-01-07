using MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private static readonly string URL_SUCCESS = "https://mtg-two.vercel.app/checkout-success";
        private static readonly string URL_ERROR = "https://mtg-two.vercel.app/checkout-fail";
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn([FromQuery] PaymentDTORequest parameters)
        {
            try
            {
                // Kiểm tra nếu là giao dịch nạp tiền vào ví
                if (parameters.vnp_OrderInfo?.StartsWith("Nap tien vi") == true)
                {
                    var success = await _paymentService.ProcessWalletDeposit(parameters);
                    if (success)
                    {
                        return Redirect(URL_SUCCESS);
                    }
                    return Redirect(URL_ERROR);
                }

                // Xử lý thanh toán đơn hàng thông thường
                if (parameters.vnp_ResponseCode == "00") // Giao dịch thành công
                {
                    var result = await _paymentService.CreatePayment(parameters);
                    if (result != null)
                    {
                        return Redirect(URL_SUCCESS);
                    }
                }
                else // Giao dịch thất bại
                {
                    var res = await _paymentService.CancelTransaction(parameters);
                    if (res != null)
                    {
                        return Redirect(URL_ERROR);
                    }
                }

                return Redirect(URL_ERROR);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("momo-return")]
        public async Task<IActionResult> MomoReturn([FromQuery] PaymentDTORequest parameters)
        {
            try
            {
                // Log để debug
                _logger.LogInformation($"MoMo return parameters: {JsonSerializer.Serialize(parameters)}");

                bool success;
                if (parameters.orderInfo.StartsWith("Nap tien vi"))
                {
                    success = await _paymentService.ProcessWalletDeposit(parameters);
                }
                else
                {
                    success = await _paymentService.ProcessMomoOrderPayment(parameters);
                }

                if (success)
                {
                    return Redirect(URL_SUCCESS);
                }

                // Log lỗi để debug
                _logger.LogError($"MoMo payment failed: {parameters.message}");
                return Redirect(URL_ERROR);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing MoMo return: {ex.Message}");
                return Redirect(URL_ERROR);
            }
        }

        [HttpPost("momo-ipn")]
        public async Task<IActionResult> MomoIPN([FromBody] PaymentDTORequest parameters)
        {
            try
            {
                bool success;
                if (parameters.orderInfo.StartsWith("Nap tien vi"))
                {
                    success = await _paymentService.ProcessWalletDeposit(parameters);
                }
                else
                {
                    success = await _paymentService.ProcessMomoOrderPayment(parameters);
                }

                if (success)
                {
                    return Ok(new { returnCode = 0, message = "success" });
                }

                return Ok(new { returnCode = 1, message = "failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing MoMo IPN: {ex.Message}");
                return Ok(new { returnCode = 99, message = "error" });
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("get-payments")]
        public async Task<IActionResult> GetPayments(DateTime startDate, DateTime endDate, int status)
        {
            try
            {
                var payments = await _paymentService.GetPaymentList(startDate, endDate, status);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("get-payment/{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentById(paymentId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("deposit-wallet")]
        [Authorize]
        public async Task<IActionResult> CreateWalletDeposit([FromBody] WalletDepositRequest request)
        {
            try
            {
                var result = await _paymentService.CreateWalletDepositPayment(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("wallet-deposit-return")]
        public async Task<IActionResult> WalletDepositReturn([FromQuery] PaymentDTORequest parameters)
        {
            try
            {
                var success = await _paymentService.ProcessWalletDeposit(parameters);
                return Redirect(success ? URL_SUCCESS : URL_ERROR);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
