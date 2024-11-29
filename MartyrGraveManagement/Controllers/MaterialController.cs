using MartyrGraveManagement_BAL.ModelViews.MaterialDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        /// <summary>
        /// Get all materials active
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMaterials()
        {
            try
            {
                var materials = await _materialService.GetAllMaterials();
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get all materials for admin
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetMaterialsForAdmin()
        {
            try
            {
                var materials = await _materialService.GetMaterialsForAdmin();
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get material by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialById(int id)
        {
            try
            {
                var material = await _materialService.GetMaterialById(id);
                if (material == null)
                    return NotFound("Material not found");
                return Ok(material);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Get material by ServiceID
        /// </summary>
        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetMaterialByServiceId(int id)
        {
            try
            {
                var material = await _materialService.GetMaterialsByServiceIdAsync(id);
                if (material == null)
                    return NotFound("Material not found");
                return Ok(material);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Create new material
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost]
        public async Task<IActionResult> CreateMaterial([FromBody] MaterialDtoRequest materialDto)
        {
            try
            {
                var (success, message) = await _materialService.CreateMaterial(materialDto);
                if (success)
                    return Ok(message);
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update material
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial(int id, [FromBody] MaterialDtoRequest materialDto)
        {
            try
            {
                var (success, message) = await _materialService.UpdateMaterial(id, materialDto);
                if (success)
                    return Ok(message);
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete material
        /// </summary>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> UpdateStatusMaterial(int id)
        {
            try
            {
                var (success, message) = await _materialService.UpdateStatus(id);
                if (success)
                    return Ok(message);
                return BadRequest(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
