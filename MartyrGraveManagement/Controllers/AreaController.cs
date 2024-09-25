using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
using MartyrGraveManagement_BAL.Services.Interfaces;
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
        /// Create a new Area.
        /// </summary>
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
        /// Get a list of Areas.
        /// </summary>
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
        /// Get an Area by ID.
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Returns the specified area.</returns>
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
        /// Update an Area.
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <param name="updateArea">Updated area data</param>
        /// <returns>Returns success message.</returns>
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
        /// Delete an Area.
        /// </summary>
        /// <param name="id">Area ID</param>
        /// <returns>Returns success message.</returns>
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
