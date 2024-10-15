using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        /// <summary>
        /// Create a new Area (Admin or Manager ROle)
        /// </summary>
        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPost("create-area")]
        public async Task<IActionResult> CreateNewArea(AreaDtoRequest areaRequest)
        {
            try
            {
                if (areaRequest == null)
                {
                    return BadRequest("Data cannot be null");
                }

                bool success = await _areaService.CreateNewArea(areaRequest);
                if (success)
                {
                    return Ok("Area created successfully.");
                }
                else
                {
                    return BadRequest("Failed to create area.");
                }
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a list of active Areas (Anonymous) - Only shows active areas with Status = true
        /// </summary>
        [AllowAnonymous]
        [HttpGet("areas")]
        public async Task<IActionResult> GetAllAreas()
        {
            try
            {
                var areas = await _areaService.GetAreas();
                return areas == null ? BadRequest("No areas found.") : Ok(areas);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all Areas for Staff or Manager (RequireManagerOrStaffRole) - Show both active and inactive areas
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("all-areas")]
        public async Task<IActionResult> GetAllAreasForStaffOrManager()
        {
            try
            {
                var areas = await _areaService.GetAllAreasForStaffOrManager();
                return areas == null ? BadRequest("No areas found.") : Ok(areas);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get an Area by ID (Anonymous)
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Returns the specified area.</returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            try
            {
                var area = await _areaService.GetAreaById(id);
                return area == null ? NotFound("Area not found.") : Ok(area);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an Area (Admin or Manager Role)
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="updateArea">Updated area data</param>
        /// <returns>Returns success message.</returns>

        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, [FromBody] AreaDtoRequest updateArea)
        {
            try
            {
                if (updateArea == null)
                {
                    return BadRequest("Data cannot be null.");
                }

                bool success = await _areaService.UpdateArea(id, updateArea);
                return success ? Ok("Area updated successfully.") : NotFound("Area not found.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update the status of an Area (Admin or Manager Role).
        /// </summary>
        /// <param name="id">The ID of the area to toggle status.</param>
        /// <returns>Returns success message.</returns>
        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatusArea(int id)
        {
            try
            {
                bool success = await _areaService.ToggleStatusArea(id);
                return success ? Ok("Area status toggled successfully.") : NotFound("Area not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        /// <summary>
        /// Delete an Area (Admin or Manager Role)
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Returns success message.</returns>
        [Authorize(Policy = "RequireManagerOrAdminRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            try
            {
                bool success = await _areaService.DeleteArea(id);
                return success ? Ok("Area deleted successfully.") : NotFound("Area not found.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
