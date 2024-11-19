using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private static readonly string URL_SUCCESS = "http://localhost:3000/checkout-success";
        private static readonly string URL_ERROR = "http://localhost:3000/checkout-fail";

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> CreatePayment([FromQuery] PaymentDTORequest parameters)
        {
            try
            {
                if (parameters.vnp_BankTranNo == null)
                {
                    var res = await _paymentService.CancelTransaction(parameters);
                    if (res != null) {
                        return Redirect(URL_ERROR);
                    } else
                    {
                        return NotFound("Order does not created");
                    }
                }
                var result = await _paymentService.CreatePayment(parameters);

                if (result != null)
                {
                    return Redirect(URL_SUCCESS);
                }
                else
                {
                    return Redirect(URL_ERROR);
                }
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
                if (parameters.resultCode != "0")
                {
                    var res = await _paymentService.CancelTransaction(parameters);
                    if (res != null) 
                    {
                        return Redirect(URL_ERROR);
                    }
                    return NotFound("Order does not created");
                }

                var result = await _paymentService.CreatePayment(parameters);
                if (result != null)
                {
                    return Redirect(URL_SUCCESS);
                }
                return Redirect(URL_ERROR);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

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
    }
}
