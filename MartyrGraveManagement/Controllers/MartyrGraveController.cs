using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
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
        public async Task<ActionResult<MartyrGraveDtoResponse>> CreateMartyrGraveV2([FromBody] MartyrGraveDtoRequest martyrGraveDto)
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

        [HttpPost("create-relative-grave-v2")]
        public async Task<IActionResult> CreateRelativeGrave([FromBody] CustomerDtoRequest customerDtoRequest, int graveId)
        {
            try
            {
                if(customerDtoRequest.UserName == null || customerDtoRequest.Phone == null)
                {
                    return BadRequest("Username or phone must be required");
                }
                var createGrave = await _martyrGraveService.CreateRelativeGraveAsync(graveId, customerDtoRequest);
                if (createGrave.status)
                {
                    return Ok(new { result = createGrave.result, accountName = createGrave.accountName, password = createGrave.password });
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
        /// Update a martyr grave status with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the martyr grave to update status.</param>
        /// <returns>Returns no content if the deletion is successful.</returns>
        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> UpdateMartyrGraveStatus(int id)
        {
            var deleted = await _martyrGraveService.UpdateStatusMartyrGraveAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok("Update Successfully");
        }


        /// <summary>
        /// Retrieves all martyr graves with associated information for management with paging support.
        /// </summary>
        /// <param name="page">The current page number (default is 1)</param>
        /// <param name="pageSize">The number of items per page (default is 10)</param>
        /// <returns>A list of martyr graves with additional information like name, location, etc.</returns>
        /// <response code="200">Returns a list of martyr graves with total pages for paging</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("GetAllForManager")]
        public async Task<IActionResult> GetAllMartyrGraves([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Gọi phương thức từ service để lấy danh sách mộ liệt sĩ có phân trang
                var graves = await _martyrGraveService.GetAllMartyrGravesForManagerAsync(page, pageSize);

                // Kiểm tra nếu dữ liệu không rỗng và trả về kết quả
                if (graves.response != null && graves.response.Any())
                {
                    return Ok(new { martyrGraveList = graves.response, totalPage = graves.totalPage });
                }
                else
                {
                    return NotFound("No martyr graves available");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a martyr grave with the specified ID (Version 2).
        /// </summary>
        /// <param name="id">The ID of the martyr grave to update.</param>
        /// <param name="martyrGraveDto">The updated details of the martyr grave.</param>
        /// <returns>Returns no content if the update is successful.</returns>
        [HttpPut("update-grave-v2/{id}")]
        public async Task<IActionResult> UpdateMartyrGraveV2(int id, MartyrGraveDtoRequest martyrGraveDto)
        {
            try
            {
                var updateGraveResult = await _martyrGraveService.UpdateMartyrGraveAsyncV2(id, martyrGraveDto);

                if (updateGraveResult.status)
                {
                    return Ok(new { result = updateGraveResult.result });
                }
                else
                {
                    return BadRequest(new { message = updateGraveResult.result });
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
