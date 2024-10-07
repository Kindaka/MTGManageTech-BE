using MartyrGraveManagement_BAL.ModelViews.JobDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        /// <summary>
        /// Get all jobs.
        /// </summary>
        /// <returns>Returns a list of all jobs.</returns>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("jobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var jobs = await _jobService.GetAllJobsAsync();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a job by ID.
        /// </summary>
        /// <param name="jobId">The ID of the job.</param>
        /// <returns>Returns the job with the specified ID.</returns>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("jobs/{jobId}")]
        public async Task<IActionResult> GetJobById(int jobId)
        {
            try
            {
                var job = await _jobService.GetJobByIdAsync(jobId);
                if (job == null)
                {
                    return NotFound("Job not found.");
                }
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new job.
        /// </summary>
        /// <param name="newJob">Details of the new job.</param>
        /// <returns>Returns the created job.</returns>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost("jobs")]
        public async Task<IActionResult> CreateJob([FromBody] JobDtoRequest newJob)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdJob = await _jobService.CreateJobAsync(newJob);
                return CreatedAtAction(nameof(GetJobById), new { jobId = createdJob.JobId }, createdJob);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing job.
        /// </summary>
        /// <param name="jobId">The ID of the job to update.</param>
        /// <param name="updatedJob">The updated job details.</param>
        /// <returns>Returns success message.</returns>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("jobs/{jobId}")]
        public async Task<IActionResult> UpdateJob(int jobId, [FromBody] JobDtoRequest updatedJob)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _jobService.UpdateJobAsync(jobId, updatedJob);
                if (!success)
                {
                    return NotFound("Job not found.");
                }

                return Ok("Job updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a job.
        /// </summary>
        /// <param name="jobId">The ID of the job to delete.</param>
        /// <returns>Returns success message.</returns>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpDelete("jobs/{jobId}")]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            try
            {
                bool success = await _jobService.DeleteJobAsync(jobId);
                if (!success)
                {
                    return NotFound("Job not found.");
                }
                return Ok("Job deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
