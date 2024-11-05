using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public SlotController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllSlots()
        {
            try
            {
                var slots = await _slotService.GetAllSlots();
                if (slots == null)
                {
                    return NotFound("Slots not avaiable");
                }
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("GetDetail/{slotId}")]
        public async Task<IActionResult> GetAllSlots(int slotId)
        {
            try
            {
                var slot = await _slotService.GetDetailSlot(slotId);
                if (slot == null)
                {
                    return NotFound("Slot not found");
                }
                return Ok(slot);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
