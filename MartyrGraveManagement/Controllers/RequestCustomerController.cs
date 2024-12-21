using MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestCustomerController : ControllerBase
    {
        private readonly IRequestCustomerService _requestCustomerService;
        private readonly IAuthorizeService _authorizeService;
        public RequestCustomerController(IRequestCustomerService requestCustomerService, IAuthorizeService authorizeService)
        {
            _requestCustomerService = requestCustomerService;
            _authorizeService = authorizeService;
        }


        /// <summary>
        /// Get a task by ID.
        /// </summary>
        [Authorize(Policy = "RequireCustomerOrManagerRole")]
        [HttpGet("requests/{requestId}")]
        public async Task<IActionResult> GetRequestsById(int requestId)
        {
            try
            {
                var request = await _requestCustomerService.GetRequestByIdAsync(requestId);

                if (request == null)
                {
                    return NotFound("Request not found.");
                }

                return Ok(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        ///// <summary>
        ///// Get all tasks for a specific martyr grave
        ///// </summary>
        //[HttpGet("martyr-grave/{martyrGraveId}")]
        //public async Task<IActionResult> GetTasksByMartyrGraveId(int martyrGraveId, int pageIndex = 1, int pageSize = 5)
        //{
        //    try
        //    {
        //        var accountId = User.FindFirst("AccountId")?.Value; // Giả sử bạn lưu userId trong token
        //        int userId;
        //        if (accountId == null)
        //        {
        //            userId = 0;
        //        }
        //        else
        //        {
        //            userId = int.Parse(accountId);
        //        }
        //        var tasks = await _taskService.GetTasksByMartyrGraveId(martyrGraveId, userId, pageIndex, pageSize);

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Tasks retrieved successfully",
        //            data = tasks.taskList,
        //            totalPage = tasks.totalPage
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            success = false,
        //            message = "Error retrieving tasks",
        //            error = ex.Message
        //        });
        //    }
        //}

        /// <summary>
        /// Get all requests by AccountId for Customer.
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("requests/account/{accountId}")]
        public async Task<IActionResult> GetTasksByAccountId(int accountId, DateTime Date, int pageIndex = 1, int pageSize = 5)
        {//
            try
            {
                var userId = int.Parse(User.FindFirst("AccountId")?.Value); // Giả sử bạn lưu userId trong token

                // Kiểm tra quyền truy cập bằng authorizeService
                var (isMatchedCustomer, isAuthorizedAccount) = await _authorizeService.CheckAuthorizeByCustomerId(userId, accountId);
                if (!isMatchedCustomer)
                {
                    return Forbid("You do not have permission to access requests for this account.");
                }

                var requests = await _requestCustomerService.GetRequestsByAccountIdAsync(accountId, pageIndex, pageSize, Date);


                return Ok(new { requests = requests.requestList, totalPage = requests.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Get requests for manager.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("requests/manager/{managerId}")]
        public async Task<IActionResult> GetRequestsBymanagerId(int managerId, DateTime Date, int pageIndex = 1, int pageSize = 5)
        {//
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeManagerByAccountId(managerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedAccountManager)
                {
                    return Forbid();
                }

                var requests = await _requestCustomerService.GetRequestsForManager(managerId, pageIndex, pageSize, Date);


                return Ok(new { requests = requests.requestList, totalPage = requests.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a request.
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("request")]
        public async Task<IActionResult> CreateRequest(RequestCustomerDtoRequest requestDtos)
        {
            try
            {
                var tokenAccountIdClaim = User.FindFirst("AccountId");
                if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
                {
                    return Forbid("Không tìm thấy AccountId trong token.");
                }

                var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);
                if (tokenAccountId != requestDtos.CustomerId)
                {
                    return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
                }
                // Gọi service để tạo task từ danh sách
                var createdRequest = await _requestCustomerService.CreateRequestsAsync(requestDtos);
                if (createdRequest.status)
                {
                    return Ok(new { message = "Requests created successfully.", createdRequest.response });
                }
                else
                {
                    return BadRequest(new { message = $"{createdRequest.response}" });
                }

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Accept a request.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("AcceptRequest")]
        public async Task<IActionResult> AcceptRequest(int requestId, int managerId, RequestMaterialDtoRequest requestMaterial)
        {
            try
            {
                var tokenAccountIdClaim = User.FindFirst("AccountId");
                if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
                {
                    return Forbid("Không tìm thấy AccountId trong token.");
                }

                var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);
                if (tokenAccountId != managerId)
                {
                    return Forbid("Bạn không có quyền cập nhật request.");
                }
                // Gọi service để tạo task từ danh sách
                var request = await _requestCustomerService.AcceptRequestForManagerAsync(requestId, managerId, requestMaterial);
                if (request.status)
                {
                    return Ok(new { message = "Requests created successfully.", request.response });
                }
                else
                {
                    return BadRequest(new { message = $"{request.response}" });
                }

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
