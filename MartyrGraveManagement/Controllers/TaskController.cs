using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        /// <returns>Returns a list of all tasks.</returns>
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
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>Returns the task with the specified ID.</returns>
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
        /// <param name="taskDto">Task data to create.</param>
        /// <returns>Returns the created task.</returns>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost("tasks")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDtoRequest taskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdTask = await _taskService.CreateTaskAsync(taskDto);
                return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTask.TaskId }, createdTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);  // Trả về lỗi nếu không tìm thấy Account hoặc Order
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);  // Trả về lỗi nếu nhân viên không có quyền làm việc
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);  // Trả về lỗi nếu có điều kiện không hợp lệ
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update task status.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="request">The request containing status update.</param>
        /// <returns>Returns the updated task with the new status.</returns>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] UpdateTaskStatusRequest request)
        {
            try
            {
                var updatedTask = await _taskService.UpdateTaskStatusAsync(taskId, request.AccountId, request.Status, request.UrlImage, request.Reason);
                return Ok(updatedTask);
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
        /// Delete a task by ID if the status is 0 (unassigned).
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>Returns a success message or an error message if the task cannot be deleted.</returns>
        [Authorize(Policy = "RequireManagerRole")]  // Only managers can delete tasks
        [HttpDelete("tasks/{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(taskId);
                if (result)
                {
                    return Ok("Task deleted successfully.");
                }
                else
                {
                    return BadRequest("Task could not be deleted or does not exist.");
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        /// Reassign a task to another staff by updating the AccountId and resetting the status to 0.
        /// </summary>
        /// <param name="taskId">The ID of the task to reassign.</param>
        /// <param name="request">The reassignment request containing the new AccountId.</param>
        /// <returns>Returns the updated task with the new assignee.</returns>
        [Authorize(Policy = "RequireManagerRole")]  // Only managers can reassign tasks
        [HttpPut("tasks/{taskId}/reassign")]
        public async Task<IActionResult> ReassignTask(int taskId, [FromBody] ReassignTaskRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTask = await _taskService.ReassignTaskAsync(taskId, request.NewAccountId);
                return Ok(updatedTask);
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
    }
}
