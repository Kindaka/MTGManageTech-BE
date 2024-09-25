using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MartyrGraveInformationController : ControllerBase
    {
        private readonly IMartyrGraveInformationService _martyrGraveInformationService;

        public MartyrGraveInformationController(IMartyrGraveInformationService martyrGraveInformationService)
        {
            _martyrGraveInformationService = martyrGraveInformationService;
        }

        /// <summary>
        /// Gets all martyr grave information.
        /// </summary>
        /// <returns>Returns a list of all martyr grave information records.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MartyrGraveInformationDtoResponse>>> GetAll()
        {
            var informationList = await _martyrGraveInformationService.GetAllAsync();
            return Ok(informationList);
        }

        /// <summary>
        /// Gets a specific martyr grave information by its ID.
        /// </summary>
        /// <param name="id">The ID of the martyr grave information.</param>
        /// <returns>Returns the martyr grave information with the specified ID.</returns>
        /// <response code="200">If the record is found</response>
        /// <response code="404">If the record is not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<MartyrGraveInformationDtoResponse>> GetById(int id)
        {
            var information = await _martyrGraveInformationService.GetByIdAsync(id);
            if (information == null)
            {
                return NotFound();
            }
            return Ok(information);
        }

        /// <summary>
        /// Creates a new martyr grave information record.
        /// </summary>
        /// <param name="martyrGraveInformationDto">The details of the martyr grave information to create.</param>
        /// <returns>Returns the created martyr grave information.</returns>
        /// <response code="201">If the creation is successful</response>
        /// <response code="404">If the MartyrId does not exist</response>
        [HttpPost]
        public async Task<ActionResult<MartyrGraveInformationDtoResponse>> Create(MartyrGraveInformationDtoRequest martyrGraveInformationDto)
        {
            try
            {
                var createdInformation = await _martyrGraveInformationService.CreateAsync(martyrGraveInformationDto);
                return CreatedAtAction(nameof(GetById), new { id = createdInformation.InformationId }, createdInformation);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates a specific martyr grave information.
        /// </summary>
        /// <param name="id">The ID of the martyr grave information to update.</param>
        /// <param name="martyrGraveInformationDto">The updated details of the martyr grave information.</param>
        /// <returns>Returns no content if the update is successful.</returns>
        /// <response code="204">If the update is successful</response>
        /// <response code="404">If the MartyrId does not exist or the record is not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MartyrGraveInformationDtoRequest martyrGraveInformationDto)
        {
            try
            {
                var updatedInformation = await _martyrGraveInformationService.UpdateAsync(id, martyrGraveInformationDto);
                if (updatedInformation == null)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a specific martyr grave information record.
        /// </summary>
        /// <param name="id">The ID of the martyr grave information to delete.</param>
        /// <returns>Returns no content if the deletion is successful.</returns>
        /// <response code="204">If the deletion is successful</response>
        /// <response code="404">If the record is not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _martyrGraveInformationService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
