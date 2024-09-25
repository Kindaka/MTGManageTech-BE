using MartyrGraveManagement_BAL.ModelViews.AreaDTos;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        public AreaController(IAreaService areaService) { 
            _areaService = areaService;
        }


        /// <summary>
        /// Create Area for Staff (Manager Role).
        /// </summary>
        [HttpPost("create-area")]
        public async Task<IActionResult> CreateNewArea(AreaDtoRequest areaRequest)
        {
            try
            {
                if (areaRequest == null)
                {
                    return BadRequest("Data cannot null");
                }
                
                bool checkSuccess = await _areaService.CreateNewArea(areaRequest);
                if (checkSuccess)
                {
                    return Ok("Create success");
                }
                else
                {
                    return BadRequest("Cannot create area");
                }
                
                
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return StatusCode(500, $"Internal Server Error: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Area list.
        /// </summary>
        [HttpGet("areas")]
        public async Task<IActionResult> GetAllArea()
        {
            try
            {


                var areas = await _areaService.GetAreas();
                if (areas != null)
                {
                    return Ok(areas);
                }
                else
                {
                    return BadRequest("Empty");
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
