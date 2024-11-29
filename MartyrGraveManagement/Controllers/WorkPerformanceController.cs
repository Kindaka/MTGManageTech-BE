using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkPerformanceController : ControllerBase
    {

        /// <summary>
        /// Create Feedback after Order Completed (Customer Role)
        /// </summary>
        //[Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("Create-performance")]
        public async Task<IActionResult> CreateWorkPerformance([FromBody] FeedbackDtoRequest feedbackDto)
        {
            return BadRequest();
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkPerformanceById(int id)
        {
            return BadRequest();
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("getbyStaffId/{staffId}")]
        public async Task<IActionResult> GetWorkPerformanceByStaffId(int staffId)
        {
            return BadRequest();
        }


        /// <summary>
        /// get all feedback (Staff or Manager Role)
        /// </summary>
        //[Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("Get-all-performance")]
        public async Task<IActionResult> GetAllWorkPerformance([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return BadRequest();
        }

        /// <summary>
        /// Update feedback content (Customer role)
        /// </summary>
        //[Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkPerformance(int id, [FromBody] FeedbackContentDtoRequest feedbackDto)
        {
            return BadRequest();
        }




        /// <summary>
        /// Delete Feedback 
        /// </summary>
        //[Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkPerformance(int id)
        {
            return BadRequest();
        }
    }
}
