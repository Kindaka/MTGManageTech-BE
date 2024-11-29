using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentTaskController : ControllerBase
    {
        private readonly IAssignmentTaskService _assignmentTaskService;

        public AssignmentTaskController(IAssignmentTaskService assignmentTaskService)
        {
            _assignmentTaskService = assignmentTaskService;
        }


        /// <summary>
        /// Get Assignment Task by StaffId (Staff Role)
        /// </summary>
        [HttpGet("staff")]
        [Authorize(Policy = "RequireStaffRole")]
        public async Task<IActionResult> GetTasksByStaffId([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] DateTime date = default)
        {
            try
            {
                var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var (tasks, totalPage) = await _assignmentTaskService.GetAssignmentTasksByAccountIdAsync(accountId, pageIndex, pageSize, date);
                return Ok(new { tasks, totalPage });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Get Assignment Task by Manager (Manager Role)
        /// </summary>
        [HttpGet("manager")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> GetTasksForManager([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] DateTime date = default)
        {
            try
            {
                var managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var (tasks, totalPage) = await _assignmentTaskService.GetAssignmentTasksForManager(managerId, pageIndex, pageSize, date);
                return Ok(new { tasks, totalPage });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Get Assignment Task by Id
        /// </summary>
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            try
            {
                var task = await _assignmentTaskService.GetAssignmentTaskByIdAsync(taskId);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Reject Task / Update Assignment Task Status 2 (Staff Role)
        /// </summary>
        [HttpPut("status/{taskId}")]
        [Authorize(Policy = "RequireStaffRole")]
        public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] AssignmentTaskStatusUpdateDTO updateDto)
        {
            try
            {
                var currentStaffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var task = await _assignmentTaskService.GetAssignmentTaskByIdAsync(taskId);
                if (task.StaffId != currentStaffId)
                {
                    return Forbid("You don't have permission to update this task.");
                }

                var updatedTask = await _assignmentTaskService.UpdateTaskStatusAsync(taskId, updateDto);
                return Ok(updatedTask);
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
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Completed Task / Update Assignment Task Status 4 (Staff Role)
        /// </summary>
        [HttpPut("images/{taskId}")]
        [Authorize(Policy = "RequireStaffRole")]
        public async Task<IActionResult> UpdateTaskImages(int taskId, [FromBody] AssignmentTaskImageUpdateDTO imageUpdateDto)
        {
            try
            {
                var currentStaffId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var task = await _assignmentTaskService.GetAssignmentTaskByIdAsync(taskId);
                if (task.StaffId != currentStaffId)
                {
                    return Forbid("You don't have permission to update this task.");
                }

                var updatedTask = await _assignmentTaskService.UpdateTaskImagesAsync(taskId, imageUpdateDto);
                return Ok(updatedTask);
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
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reassign Task to another staff (Manager Role)
        /// </summary>
        [HttpPut("reassign/{taskId}")]
        [Authorize(Policy = "RequireManagerRole")]
        public async Task<IActionResult> ReassignTask(int taskId, [FromQuery] int newStaffId)
        {
            try
            {
                var updatedTask = await _assignmentTaskService.ReassignTaskAsync(taskId, newStaffId);
                return Ok(updatedTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
