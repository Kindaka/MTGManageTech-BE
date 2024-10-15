using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Get all tasks.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
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
        /// Get tasks by AccountId.
        /// </summary>
        /// <param name="accountId">The ID of the account.</param>
        /// <returns>Returns a list of tasks assigned to the specified account.</returns>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("tasks/account/{accountId}")]
        public async Task<IActionResult> GetTasksByAccountId(int accountId)
        {
            try
            {
                var tasks = await _taskService.GetTasksByAccountIdAsync(accountId);
                if (tasks == null || !tasks.Any())
                {
                    return NotFound("No tasks found for the specified account.");
                }
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new task.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost("tasks")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDtoRequest taskDto)
        {
            try
            {
                // Lấy ManagerId từ JWT token
                var managerId = int.Parse(User.FindFirst("accountId")?.Value);

                // Tạo task với ManagerId
                var createdTask = await _taskService.CreateTaskAsync(taskDto, managerId);

                return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTask.TaskId }, createdTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Assign a task to a staff member (Manager Role).
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("tasks/{taskId}/assign")]
        public async Task<IActionResult> AssignTask(int taskId, [FromBody] AssignTaskDTORequest assignRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Lấy accountId từ JWT token
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Forbid("Token không chứa thông tin accountId.");
                }

                var accountIdFromToken = int.Parse(accountIdClaim);

                // Thực hiện việc gán task
                var result = await _taskService.AssignTaskAsync(taskId, assignRequest.StaffId);

                return Ok(new { message = "Task assigned successfully.", result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
        /// Update task status (Staff Role).
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] UpdateTaskStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var accountIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Kiểm tra xem accountId trong token và request có khớp không
                if (accountIdFromToken != request.AccountId)
                {
                    return Forbid("Bạn không có quyền chỉnh sửa task cho account này.");
                }

                // Tạo danh sách các URL hình ảnh
                var urlImages = new List<string>();
                if (!string.IsNullOrEmpty(request.UrlImage1)) urlImages.Add(request.UrlImage1);
                if (!string.IsNullOrEmpty(request.UrlImage2)) urlImages.Add(request.UrlImage2);
                if (!string.IsNullOrEmpty(request.UrlImage3)) urlImages.Add(request.UrlImage3);

                // Gọi service để cập nhật trạng thái task
                var updatedTask = await _taskService.UpdateTaskStatusAsync(taskId, request.AccountId, request.Status, urlImages, request.Reason);
                return Ok(new { message = "Task status updated successfully.", updatedTask });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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
        /// <param name="request">The reassignment request containing the new AccountId.</param>
        /// <returns>Returns the updated task with the new assignee.</returns>
        [Authorize(Policy = "RequireManagerRole")]  // Chỉ Manager mới được phép bàn giao task
        [HttpPut("tasks/{taskId}/reassign")]
        public async Task<IActionResult> ReassignTask(int taskId, [FromBody] ReassignTaskRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Lấy accountId từ JWT token
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(accountIdClaim))
                {
                    return Forbid("Token không chứa thông tin accountId.");
                }

                var accountIdFromToken = int.Parse(accountIdClaim);

                // Gọi service để thực hiện gán lại task
                var updatedTask = await _taskService.ReassignTaskAsync(taskId, request.NewAccountId);
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
