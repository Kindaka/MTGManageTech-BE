using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackResponseController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackResponseController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Create Feedback after Order Completed (Customer Role)
        /// </summary>
        //[Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("Create-Feedback-Response")]
        public async Task<IActionResult> CreateFeedbackResponse([FromBody] FeedbackDtoRequest feedbackDto)
        {
            return BadRequest();
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetFeedbackResponseById(int id)
        {
            return BadRequest();
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("getByFeedbackId/{feedbackId}")]
        public async Task<IActionResult> GetFeedbackResponseByFeedbackId(int feedbackId)
        {
            return BadRequest();
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("getByCustomerId/{customerId}")]
        public async Task<IActionResult> GetFeedbackResponseByCustomerId(int customerId)
        {
            return BadRequest();
        }


        /// <summary>
        /// get all feedback (Staff or Manager Role)
        /// </summary>
        //[Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("Get-all-feedback-response")]
        public async Task<IActionResult> GetAllFeedbackResponses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return BadRequest();
        }

        /// <summary>
        /// Update feedback content (Customer role)
        /// </summary>
        //[Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedbackResponse(int id, [FromBody] FeedbackContentDtoRequest feedbackDto)
        {
            return BadRequest();
        }

        /// <summary>
        /// Update feedback status (ManagerOrStaffRole)
        /// </summary>       
        //[Authorize(Policy = "RequireManagerOrStaffRole")]  // Chỉ cho phép Admin hoặc Manager thay đổi trạng thái
        [HttpPut("{id}/change-status")]
        public async Task<IActionResult> ChangeStatusFeedbackResponse(int id)
        {
            return BadRequest();
        }



        /// <summary>
        /// Delete Feedback 
        /// </summary>
        //[Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedbackResponse(int id)
        {
            return BadRequest();
        }
    }
}
