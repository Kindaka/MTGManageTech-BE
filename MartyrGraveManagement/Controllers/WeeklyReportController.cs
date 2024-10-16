using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeeklyReportController : ControllerBase
    {
        /// <summary>
        /// Create Feedback after Order Completed (Customer Role)
        /// </summary>
        //[Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("Create-weeklyReport")]
        public async Task<IActionResult> CreateWeeklyReport([FromBody] FeedbackDtoRequest feedbackDto)
        {
            return BadRequest();
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeeklyReportById(int id)
        {
            return BadRequest();
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("getbyMartyrId/{martyrId}")]
        public async Task<IActionResult> GetWeeklyReportByMartyrId(int staffId)
        {
            return BadRequest();
        }


        /// <summary>
        /// get all feedback (Staff or Manager Role)
        /// </summary>
        //[Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("Get-Weekly-Report")]
        public async Task<IActionResult> GetAllWeeklyReport([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return BadRequest();
        }

        /// <summary>
        /// Update feedback content (Customer role)
        /// </summary>
        //[Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeeklyReport(int id, [FromBody] FeedbackContentDtoRequest feedbackDto)
        {
            return BadRequest();
        }




        /// <summary>
        /// Delete Feedback 
        /// </summary>
        //[Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeeklyReport(int id)
        {
            return BadRequest();
        }
    }
}
