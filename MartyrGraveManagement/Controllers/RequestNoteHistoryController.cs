using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestNoteHistoryController : ControllerBase
    {
        private readonly IRequestNoteHistoryService _requestNoteHistoryService;

        public RequestNoteHistoryController(IRequestNoteHistoryService requestNoteHistoryService)
        {
            _requestNoteHistoryService = requestNoteHistoryService;
        }

        /// <summary>
        /// Get all RequestNoteHistory
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _requestNoteHistoryService.GetAllRequestNoteHistoriesAsync();
            return Ok(result);
        }
    }
}
