using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringServiceScheduleController : ControllerBase
    {
        private readonly IServiceSchedule_Service _serviceScheduleService;
        private readonly IAuthorizeService _authorizeService;

        public RecurringServiceScheduleController(IServiceSchedule_Service serviceScheduleService, IAuthorizeService authorizeService)
        {
            _serviceScheduleService = serviceScheduleService;
            _authorizeService = authorizeService;
        }

        // POST: api/RecurringServiceSchedule
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<ActionResult> CreateServiceSchedule(ServiceScheduleDtoRequest request)
        {
            try
            {
                if (request.DayOfService <= 0)
                {
                    return BadRequest("Không thể thêm ngày nhỏ hơn 0");
                }
                if (request == null)
                {
                    return BadRequest("Cannot add empty object to cart");
                }
                // Lấy AccountId từ thông tin đăng nhập của người dùng
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }

                // Kiểm tra tất cả các mục trong danh sách có hợp lệ với AccountId của người dùng không

                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(request.AccountId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }




                // Gọi service để tạo danh sách CartItems và lấy kết quả
                var (status, messages) = await _serviceScheduleService.CreateServiceSchedule(request);

                // Trả về kết quả bao gồm danh sách phản hồi và thông báo
                return Ok(new { status, messages });
            }
            catch (KeyNotFoundException ex)
            {
                // Xử lý lỗi không tìm thấy dữ liệu
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Xử lý lỗi liên quan đến logic kinh doanh (vd: GraveService không tồn tại)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi không mong muốn
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("GetServiceSchedulesForCustomer/{customerId}")]
        public async Task<ActionResult<ServiceScheduleDtoResponse>> GetServiceSchedulesForCustomer(int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }
                var result = await _serviceScheduleService.GetServiceScheduleByAccountId(customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("GetServiceScheduleById/{Id}")]
        public async Task<ActionResult<ServiceScheduleDetailResponse>> GetServiceScheduleById(int Id, int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }
                var result = await _serviceScheduleService.GetServiceScheduleById(Id);
                if (result == null)
                {
                    return NotFound(new { message = "Service schedule not found." });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("UpdateServiceSchedule/{Id}")]
        public async Task<ActionResult<ServiceScheduleDetailResponse>> UpdateStatusServiceSchedule(int Id, int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }
                var result = await _serviceScheduleService.UpdateStatusServiceSchedule(Id, customerId);
                if (result == false)
                {
                    return NotFound(new { message = "Service schedule update fail." });
                }
                else
                {
                    return Ok(new { message = "Update sucessfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
