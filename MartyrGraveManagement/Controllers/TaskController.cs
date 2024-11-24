using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IAuthorizeService _authorizeService;
        public TaskController(ITaskService taskService, IAuthorizeService authorizeService)
        {
            _taskService = taskService;
            _authorizeService = authorizeService;
        }

        /// <summary>
        /// Get all tasks.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {//
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a task by ID.
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("tasks/{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(taskId);

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
        public async Task<IActionResult> GetTasksByMartyrGraveId(int martyrGraveId, int userId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByMartyrGraveId(martyrGraveId, userId);

                return Ok(new
                {
                    success = true,
                    message = "Tasks retrieved successfully",
                    data = tasks
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
        [HttpGet("tasks/account/{accountId}")]
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

                var tasks = await _taskService.GetTasksByAccountIdAsync(accountId, pageIndex, pageSize, Date);


                return Ok(new {tasks = tasks.taskList, totalPage = tasks.totalPage});
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
        [HttpGet("tasksNotScheduling/account/{accountId}")]
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

                var tasks = await _taskService.GetTasksNotSchedulingByAccountIdAsync(accountId, pageIndex, pageSize, Date);


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
        [HttpGet("tasks/manager/{managerId}")]
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

                var tasks = await _taskService.GetTasksForManager(managerId, pageIndex, pageSize, Date);


                return Ok(new { tasks = tasks.taskList, totalPage = tasks.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a list of tasks.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost("tasks")]
        public async Task<IActionResult> CreateTask([FromBody] List<TaskDtoRequest> taskDtos)
        {
            try
            {
                // Gọi service để tạo task từ danh sách
                var createdTasks = await _taskService.CreateTasksAsync(taskDtos);

                return Ok(new { message = "Tasks created successfully.", createdTasks });
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
        /// Update task status (Staff Role). (Status 2 Reject)
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("tasks/{taskId}/status/{newStatus}")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, int newStatus)
        {
            try
            {
                // Lấy AccountId và AreaId của người dùng từ JWT token
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var areaIdClaim = User.FindFirst("areaId")?.Value;

                if (string.IsNullOrEmpty(accountIdClaim) || string.IsNullOrEmpty(areaIdClaim))
                {
                    return Forbid("Token does not contain necessary account information.");
                }

                var accountIdFromToken = int.Parse(accountIdClaim);
                var areaIdFromToken = int.Parse(areaIdClaim);

                // Kiểm tra xem nhân viên có được phép làm việc trong khu vực của task và có phải là người sở hữu task không
                var isAuthorized = await _authorizeService.CheckAuthorizeStaffByAreaId(taskId, accountIdFromToken, areaIdFromToken);
                if (!isAuthorized)
                {
                    return Forbid("Staff is not authorized to update the status of this task.");
                }

                // Gọi service để cập nhật trạng thái của task
                var updatedTask = await _taskService.UpdateTaskStatusAsync(taskId, newStatus);

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
        [HttpPut("tasks/{taskId}/images")]
        public async Task<IActionResult> UpdateTaskImages(int taskId, [FromBody] TaskImageUpdateDTO imageUpdateDto)
        {
            try
            {
                // Lấy AccountId và AreaId của người dùng từ JWT token
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var areaIdClaim = User.FindFirst("areaId")?.Value;

                if (string.IsNullOrEmpty(accountIdClaim) || string.IsNullOrEmpty(areaIdClaim))
                {
                    return Forbid("Token does not contain necessary account information.");
                }

                var accountIdFromToken = int.Parse(accountIdClaim);
                var areaIdFromToken = int.Parse(areaIdClaim);

                // Kiểm tra xem nhân viên có được phép làm việc trong khu vực của task không
                var isAuthorized = await _authorizeService.CheckAuthorizeStaffByAreaId(taskId, accountIdFromToken, areaIdFromToken);
                if (!isAuthorized)
                {
                    return Forbid("Staff is not authorized to update images for this task in this area.");
                }

                // Gọi service để cập nhật hình ảnh của task
                var updatedTask = await _taskService.UpdateTaskImagesAsync(taskId, imageUpdateDto);

                return Ok(new { message = "Task images updated successfully.", updatedTask });
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
        /// Delete a task by ID if the status is 0 (unassigned).
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>Returns a success message or an error message if the task cannot be deleted.</returns>
        [Authorize(Policy = "RequireManagerRole")]  // Chỉ Manager mới được phép xóa task
        [HttpDelete("tasks/{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            try
            {
                // Gọi service để xóa task
                var result = await _taskService.DeleteTaskAsync(taskId);

                if (result)
                {
                    return Ok(new { message = "Task deleted successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Task could not be deleted or does not exist." });
                }
            }
            catch (KeyNotFoundException ex)
            {
                // Trả về lỗi nếu không tìm thấy TaskId
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Trả về lỗi nếu task không hợp lệ để xóa
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Trả về lỗi chung cho các lỗi khác
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


        /// <summary>
        /// Reassign a task to another staff by updating the AccountId and resetting the status to 1.
        /// </summary>
        /// <param name="taskId">The ID of the task to reassign.</param>
        /// <param name="newAccountId">The new AccountId of the staff to assign the task to.</param>
        /// <returns>Returns the updated task with the new assignee.</returns>
        [Authorize(Policy = "RequireManagerRole")]  // Chỉ Manager mới được phép bàn giao task
        [HttpPut("tasks/{detailId}/reassign/{newAccountId}")]
        public async Task<IActionResult> ReassignTask(int detailId, int newAccountId)
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

                var updatedTask = await _taskService.ReassignTaskAsync(detailId, newAccountId);
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
