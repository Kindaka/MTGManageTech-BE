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
        [Authorize(Policy = "RequireManagerOrStaffRole")]
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
        /// Create a new task.
        /// </summary>
        /// <param name="taskDto">Task data to create.</param>
        /// <returns>Returns the created task.</returns>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
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


    }
}
