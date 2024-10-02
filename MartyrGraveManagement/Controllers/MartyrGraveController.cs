using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MartyrGraveController : ControllerBase
    {
        private readonly IMartyrGraveService _martyrGraveService;

        public MartyrGraveController(IMartyrGraveService martyrGraveService)
        {
            _martyrGraveService = martyrGraveService;
        }

        /// <summary>
        /// Gets all martyr graves.
        /// </summary>
        /// <returns>Returns a list of all graves.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MartyrGraveDtoResponse>>> GetMartyrGraves()
        {
            var graves = await _martyrGraveService.GetAllMartyrGravesAsync();
            return Ok(graves);
        }

        /// <summary>
        /// Gets a specific martyr grave by its ID.
        /// </summary>
        /// <param name="id">The ID of the martyr grave.</param>
        /// <returns>Returns the martyr grave with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MartyrGraveDtoResponse>> GetMartyrGrave(int id)
        {
            var grave = await _martyrGraveService.GetMartyrGraveByIdAsync(id);
            if (grave == null)
            {
                return NotFound();
            }
            return Ok(grave);
        }

        /// <summary>
        /// Creates a new martyr grave.
        /// </summary>
        /// <param name="martyrGraveDto">The details of the martyr grave to create.</param>
        /// <returns>Returns the created martyr grave.</returns>
        [HttpPost]
        public async Task<ActionResult<MartyrGraveDtoResponse>> CreateMartyrGrave(MartyrGraveDtoRequest martyrGraveDto)
        {
            try
            {
                var createdGrave = await _martyrGraveService.CreateMartyrGraveAsync(martyrGraveDto);
                return CreatedAtAction(nameof(GetMartyrGrave), new { id = createdGrave.MartyrId }, createdGrave);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new martyr grave version 2.
        /// </summary>
        /// <param name="martyrGraveDto">The details of the martyr grave to create.</param>
        /// <returns>Returns no content if the create is successful.</returns>
        [HttpPost("create-grave-v2")]
        public async Task<ActionResult<MartyrGraveDtoResponse>> CreateMartyrGraveV2(MartyrGraveDtoRequest martyrGraveDto)
        {
            try
            {
                var createGrave = await _martyrGraveService.CreateMartyrGraveAsyncV2(martyrGraveDto);
                if(createGrave.status)
                {
                    return Ok(new { result = createGrave.result, accountName = createGrave.accountName, password = createGrave.password});
                }
                else
                {
                    return BadRequest($"{createGrave.result}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a martyr grave with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the martyr grave to update.</param>
        /// <param name="martyrGraveDto">The updated details of the martyr grave.</param>
        /// <returns>Returns no content if the update is successful.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMartyrGrave(int id, MartyrGraveDtoRequest martyrGraveDto)
        {
            try
            {
                var updatedGrave = await _martyrGraveService.UpdateMartyrGraveAsync(id, martyrGraveDto);
                if (updatedGrave == null)
                {
                    return NotFound();
                }
                return Ok("Update Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a martyr grave with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the martyr grave to delete.</param>
        /// <returns>Returns no content if the deletion is successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMartyrGrave(int id)
        {
            var deleted = await _martyrGraveService.DeleteMartyrGraveAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok("Delete Successfully");
        }


        /// <summary>
        /// Retrieves all martyr graves with associated information.
        /// </summary>
        /// <returns>A list of martyr graves with additional information like name, location, etc.</returns>
        /// <response code="200">Returns a list of martyr graves</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("GetAllForManager")]
        public async Task<ActionResult<IEnumerable<MartyrGraveGetAllDtoResponse>>> GetAllMartyrGraves()
        {
            try
            {
                var graves = await _martyrGraveService.GetAllMartyrGravesForManagerAsync();
                return Ok(graves);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
