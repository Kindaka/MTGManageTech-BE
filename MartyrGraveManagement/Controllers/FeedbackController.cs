﻿using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Create Feedback after Order Completed (Customer Role)
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost("Create-Feedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDtoRequest feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy accountId từ token
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return Forbid("Token does not contain accountId.");
            }

            int accountIdFromToken = int.Parse(accountIdClaim);

            // Kiểm tra accountId từ token có khớp với accountId từ request không
            if (accountIdFromToken != feedbackDto.AccountId)
            {
                return Forbid("You can only create feedback for your own orders.");
            }

            var result = await _feedbackService.CreateFeedbackAsync(feedbackDto);
            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(result.feedback);
        }

        /// <summary>
        /// get feedback by Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var result = await _feedbackService.GetFeedbackByIdAsync(id);

            if (!result.success)
            {
                return NotFound(result.message);
            }

            return Ok(result.feedback);  // Trả về đối tượng feedback đã chứa FullName và CustomerCode
        }


        /// <summary>
        /// get all feedback (Staff or Manager Role)
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]  
        [HttpGet("Get-all-feedback")]
        public async Task<IActionResult> GetAllFeedbacks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _feedbackService.GetAllFeedbacksAsync(page, pageSize);
            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(new { feedbacks = result.feedbacks, totalPage = result.totalPage });
        }

        /// <summary>
        /// Update feedback content (Customer role)
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackContentDtoRequest feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _feedbackService.UpdateFeedbackAsync(id, feedbackDto);
            if (!result.success)
            {
                return NotFound(result.message);
            }

            return Ok(result.message);
        }

        /// <summary>
        /// Update feedback status (ManagerOrStaffRole)
        /// </summary>       
        [Authorize(Policy = "RequireManagerOrStaffRole")]  // Chỉ cho phép Admin hoặc Manager thay đổi trạng thái
        [HttpPut("{id}/change-status")]
        public async Task<IActionResult> ChangeStatusFeedback(int id)
        {
            var result = await _feedbackService.ChangeStatusFeedbackAsync(id);
            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(result.message);
        }



        /// <summary>
        /// Delete Feedback 
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var result = await _feedbackService.DeleteFeedbackAsync(id);
            if (!result.success)
            {
                return NotFound(result.message);
            }

            return Ok(result.message);
        }


    }
}