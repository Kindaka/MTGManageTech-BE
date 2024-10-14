using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("/staffs")]
        public async Task<ActionResult<IEnumerable<AccountDtoResponse>>> GetStaffs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var staffs = await _accountService.GetStaffList(page, pageSize);
                return Ok(new { staffList = staffs.staffList, totalPage = staffs.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("/managers")]
        public async Task<ActionResult<IEnumerable<AccountDtoResponse>>> GetManagers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var managers = await _accountService.GetManagerList(page, pageSize);
                return Ok(new { managerList = managers.managerList, totalPage = managers.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("/updateStatus/{accountId}")]
        public async Task<IActionResult> UpdateStatus(int accountId)
        {
            try
            {
                var check = await _accountService.ChangeStatusUser(accountId);
                if (check)
                {
                    return Ok("Thay đổi trạng thái user thành công");
                }
                else
                {
                    return BadRequest("Không tìm thấy account");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
