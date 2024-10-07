using MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        // POST: api/Feedback
        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDtoRequest feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _feedbackService.CreateFeedbackAsync(feedbackDto);
            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(result.feedback);
        }

        // GET: api/Feedback/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var result = await _feedbackService.GetFeedbackByIdAsync(id);
            if (!result.success)
            {
                return NotFound(result.message);
            }

            return Ok(result.feedback);
        }

        // GET: api/Feedback
        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var result = await _feedbackService.GetAllFeedbacksAsync();
            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(result.feedbacks);
        }

        // PUT: api/Feedback/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackDtoRequest feedbackDto)
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

        // DELETE: api/Feedback/{id}
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
