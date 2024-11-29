using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IAuthorizeService _authorizeService;

        public WalletController(IWalletService walletService, IAuthorizeService authorizeService)
        {
            _walletService = walletService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] int customerId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var userAccountId = int.Parse(User.FindFirst("accountId")?.Value ?? "0");

                var (isMatchedCustomer, isAuthorizedAccount) = await _authorizeService
                    .CheckAuthorizeCustomerWallet(customerId, userAccountId);

                if (!isMatchedCustomer && !isAuthorizedAccount)
                {
                    return Forbid("You don't have permission to view this wallet's transactions.");
                }

                var (transactions, totalPages) = await _walletService.GetTransactionHistory(
                    customerId, pageIndex, pageSize, startDate, endDate);
                return Ok(new { transactions, totalPages });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance([FromQuery] int customerId)
        {
            try
            {
                var userAccountId = int.Parse(User.FindFirst("accountId")?.Value ?? "0");

                var (isMatchedCustomer, isAuthorizedAccount) = await _authorizeService
                    .CheckAuthorizeCustomerWallet(customerId, userAccountId);

                if (!isMatchedCustomer && !isAuthorizedAccount)
                {
                    return Forbid("You don't have permission to view this wallet.");
                }

                var wallet = await _walletService.GetCustomerWallet(customerId);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
