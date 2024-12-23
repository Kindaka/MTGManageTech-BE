using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestTaskController : ControllerBase
    {
        private readonly IRequestTaskService _requestTaskService;
        private readonly IAuthorizeService _authorizeService;
        public RequestTaskController(IRequestTaskService requestTaskService, IAuthorizeService authorizeService)
        {
            _requestTaskService = requestTaskService;
            _authorizeService = authorizeService;
        }


        /// <summary>
        /// Get a task by ID.
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("requestTasks/{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            try
            {
                var task = await _requestTaskService.GetTaskByIdAsync(taskId);

                if (task == null)
                {
                    return NotFound("Task not found.");
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all tasks for a specific martyr grave
        /// </summary>
        [HttpGet("martyr-grave/{martyrGraveId}")]
        public async Task<IActionResult> GetTasksByMartyrGraveId(int martyrGraveId, int pageIndex = 1, int pageSize = 5)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value; // Giả sử bạn lưu userId trong token
                int userId;
                if (accountId == null)
                {
                    userId = 0;
                }
                else
                {
                    userId = int.Parse(accountId);
                }
                var tasks = await _requestTaskService.GetTasksByMartyrGraveId(martyrGraveId, userId, pageIndex, pageSize);

                return Ok(new
                {
                    success = true,
                    message = "Tasks retrieved successfully",
                    data = tasks.taskList,
                    totalPage = tasks.totalPage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving tasks",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get all tasks by AccountId for staff.
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("requestTasks/account/{accountId}")]
        public async Task<IActionResult> GetTasksByAccountId(int accountId, DateTime Date, int pageIndex = 1, int pageSize = 5)
        {//
            try
            {
                var userId = int.Parse(User.FindFirst("AccountId")?.Value); // Giả sử bạn lưu userId trong token

                // Kiểm tra quyền truy cập bằng authorizeService
                var (isMatchedStaff, isAuthorizedAccount) = await _authorizeService.CheckAuthorizeStaffByAccountId(userId, accountId);
                if (!isMatchedStaff)
                {
                    return Forbid("You do not have permission to access tasks for this account.");
                }

                var tasks = await _requestTaskService.GetTasksByAccountIdAsync(accountId, pageIndex, pageSize, Date);


                return Ok(new { tasks = tasks.taskList, totalPage = tasks.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get tasks not scheduling (task status 1) by AccountId for staff.
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("requestTasksNotScheduling/account/{accountId}")]
        public async Task<IActionResult> GetTasksNotSchedulingByAccountId(int accountId, DateTime Date, int pageIndex = 1, int pageSize = 5)
        {//
            try
            {
                var userId = int.Parse(User.FindFirst("AccountId")?.Value); // Giả sử bạn lưu userId trong token

                // Kiểm tra quyền truy cập bằng authorizeService
                var (isMatchedStaff, isAuthorizedAccount) = await _authorizeService.CheckAuthorizeStaffByAccountId(userId, accountId);
                if (!isMatchedStaff)
                {
                    return Forbid("You do not have permission to access tasks for this account.");
                }

                var tasks = await _requestTaskService.GetTasksNotSchedulingByAccountIdAsync(accountId, pageIndex, pageSize, Date);


                return Ok(new { tasks = tasks.taskList, totalPage = tasks.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get tasks for manager.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("requestTasks/manager/{managerId}")]
        public async Task<IActionResult> GetTasksBymanagerId(int managerId, DateTime Date, int pageIndex = 1, int pageSize = 5)
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

                var tasks = await _requestTaskService.GetTasksForManager(managerId, pageIndex, pageSize, Date);


                return Ok(new { tasks = tasks.taskList, totalPage = tasks.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        /// <summary>
        /// Update task status (Staff Role). (Status 2 Reject)
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("requestTasks/status/{taskId}")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] AssignmentTaskStatusUpdateDTO updateDto)
        {
            try
            {
                var currentStaffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


                // Gọi service để cập nhật trạng thái của task
                var updatedTask = await _requestTaskService.UpdateTaskStatusAsync(taskId, updateDto, currentStaffId);

                return Ok(new { message = "Task status updated successfully.", updatedTask });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }



        /// <summary>
        /// Add Image task when status = 3 Processing (Staff Role) If Order have 2 Task => 2 Tasks must have status 4 and from there Order Status also up to 4
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("requestTasks/images/{taskId}")]
        public async Task<IActionResult> UpdateTaskImages(int taskId, [FromBody] TaskImageUpdateDTO imageUpdateDto)
        {
            try
            {
                // Lấy AccountId và AreaId của người dùng từ JWT token
                var currentStaffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Gọi service để cập nhật hình ảnh của task
                var updatedTask = await _requestTaskService.UpdateTaskImagesAsync(taskId, imageUpdateDto, currentStaffId);

                if (updatedTask)
                {
                    return Ok(new { message = "Task images updated successfully." });
                }
                else
                {
                    return Forbid("Update fail");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        /// <summary>
        /// Reassign a task to another staff by updating the AccountId and resetting the status to 1.
        /// </summary>
        /// <param name="taskId">The ID of the task to reassign.</param>
        /// <param name="newAccountId">The new AccountId of the staff to assign the task to.</param>
        /// <returns>Returns the updated task with the new assignee.</returns>
        [Authorize(Policy = "RequireManagerRole")]  // Chỉ Manager mới được phép bàn giao task
        [HttpPut("requestTasks/{requestTaskId}/reassign/{newAccountId}")]
        public async Task<IActionResult> ReassignTask(int requestTaskId, int newAccountId)
        {
            try
            {
                // Lấy accountId từ JWT token
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Forbid("Token không chứa thông tin accountId.");
                }

                var accountIdFromToken = int.Parse(accountIdClaim);

                var updatedTask = await _requestTaskService.ReassignTaskAsync(requestTaskId, newAccountId);
                return Ok(new { message = "Task reassigned successfully.", updatedTask });
            }
            catch (KeyNotFoundException ex)
            {
                // Nếu không tìm thấy TaskId hoặc AccountId, trả về lỗi NotFound
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                // Nếu account không có quyền thực hiện hành động này, trả về lỗi Forbid
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                // Nếu có lỗi logic trong quá trình reassignment, trả về BadRequest
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Lỗi chung khác, trả về Internal Server Error
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
