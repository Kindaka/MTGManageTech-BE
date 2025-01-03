using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MartyrGraveController : ControllerBase
    {
        private readonly IMartyrGraveService _martyrGraveService;
        private readonly IAuthorizeService _authorizeService;

        public MartyrGraveController(IMartyrGraveService martyrGraveService, IAuthorizeService authorizeService)
        {
            _martyrGraveService = martyrGraveService;
            _authorizeService = authorizeService;
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> SearchMartyrGraves([FromQuery] MartyrGraveSearchDtoRequest searchCriteria, [FromQuery] int page = 1, [FromQuery] int pageSize = 15)
        {
            try
            {
                var results = await _martyrGraveService.SearchMartyrGravesAsync(searchCriteria, page, pageSize);
                if (results == null || !results.Any())
                {
                    return NotFound("No results found.");
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        /// <summary>
        /// Gets all martyr graves.
        /// </summary>
        /// <returns>Returns a list of all graves.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MartyrGraveDtoResponse>>> GetMartyrGraves([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var graves = await _martyrGraveService.GetAllMartyrGravesAsync(page, pageSize);
                return Ok(new { graveList = graves.matyrGraveList, totalPage = graves.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /// <summary>
        /// Get all tasks for a specific martyr grave
        /// </summary>
        [HttpGet("getTasks-martyr-grave/{martyrGraveId}")]
        public async Task<IActionResult> GetTasksByMartyrGraveId(int martyrGraveId, int taskType, int pageIndex = 1, int pageSize = 5)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value; // Giả sử bạn lưu userId trong token
                int userId;
                if (accountId == null)
                {
                    userId = 0;
                }
                else
                {
                    userId = int.Parse(accountId);
                }
                var tasks = await _martyrGraveService.GetMaintenanceHistoryInMartyrGrave(userId, martyrGraveId, taskType, pageIndex, pageSize);

                return Ok(new
                {
                    success = true,
                    message = "Tasks retrieved successfully",
                    data = tasks.maintenanceHistory,
                    totalPage = tasks.totalPage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving tasks",
                    error = ex.Message
                });
            }
        }


        /// <summary>
        /// Gets a specific martyr grave by its ID.
        /// </summary>
        /// <param name="id">The ID of the martyr grave.</param>
        /// <returns>Returns the martyr grave with the specified ID.</returns>
        [AllowAnonymous]
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
        /// Gets a specific martyr grave by its customerCode.
        /// </summary>
        /// <param name="id">The customerCode of the martyr grave.</param>
        /// <returns>Returns the martyr grave with the specified customerCode.</returns>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("getMartyrGraveByCustomerId/{customerId}")]
        public async Task<ActionResult<IEnumerable<MartyrGraveDtoResponse>>> GetMartyrGraveByCustomerId(int customerId)
        {
            var graves = await _martyrGraveService.GetMartyrGraveByCustomerId(customerId);
            if (graves == null)
            {
                return NotFound();
            }
            return Ok(graves);
        }

        /// <summary>
        /// Creates a new martyr grave.
        /// </summary>
        /// <param name="martyrGraveDto">The details of the martyr grave to create.</param>
        /// <returns>Returns the created martyr grave.</returns>
        //[HttpPost]
        //public async Task<ActionResult<MartyrGraveDtoResponse>> CreateMartyrGrave(MartyrGraveDtoRequest martyrGraveDto)
        //{
        //    try
        //    {
        //        var createdGrave = await _martyrGraveService.CreateMartyrGraveAsync(martyrGraveDto);
        //        return CreatedAtAction(nameof(GetMartyrGrave), new { id = createdGrave.MartyrId }, createdGrave);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}


        /// <summary>
        /// Import excel file to add martyr graves.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <returns>Returns file path that stores customer phone number and password.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("import-graves")]
        public async Task<ActionResult<MartyrGraveDtoResponse>> ImportMartyrGraves(IFormFile file, [FromQuery] string folderPath)
        {
            if (file == null || file.Length == 0 || folderPath == null)
                return BadRequest("No file uploaded.");

            var filePath = Path.GetTempFileName(); // Temporary file path for processing

            try
            {
                // Save the file temporarily
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                // Create the full output file path by appending a filename to the selected folder path
                var outputFilePath = Path.Combine(folderPath, "MartyrGrave_Accounts.xlsx");
                // Import locations from the Excel file
                var (status, message) = await _martyrGraveService.ImportMartyrGraves(filePath, outputFilePath);

                if (status)
                {
                    return Ok(new { message = "File imported successfully.", details = message });
                }
                else
                {
                    return BadRequest(new { message = "Error importing file.", details = message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error.", details = ex.Message });
            }
            finally
            {
                // Delete the temporary file
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }


        /// <summary>
        /// Creates a new martyr grave version 2.
        /// </summary>
        /// <param name="martyrGraveDto">The details of the martyr grave to create.</param>
        /// <returns>Returns no content if the create is successful.</returns>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost("create-grave-v2")]
        public async Task<ActionResult<MartyrGraveDtoResponse>> CreateMartyrGraveV2([FromBody] MartyrGraveDtoRequest martyrGraveDto)
        {
            try
            {
                var createGrave = await _martyrGraveService.CreateMartyrGraveAsyncV2(martyrGraveDto);
                if (createGrave.status)
                {
                    return Ok(new { result = createGrave.result, phone = createGrave.phone, password = createGrave.password });
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
        //[Authorize(Policy = "RequireManagerRole")]
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateMartyrGrave(int id, MartyrGraveDtoRequest martyrGraveDto)
        //{
        //    try
        //    {
        //        var updatedGrave = await _martyrGraveService.UpdateMartyrGraveAsync(id, martyrGraveDto);
        //        if (updatedGrave == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok("Update Successfully");
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //}

        /// <summary>
        /// Update a martyr grave status with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the martyr grave to update status.</param>
        /// <returns>Returns no content if the deletion is successful.</returns>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("updateStatus/{id}")]
        public async Task<IActionResult> UpdateMartyrGraveStatus(int id, int status)
        {
            var deleted = await _martyrGraveService.UpdateStatusMartyrGraveAsync(id, status);
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
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("GetAllForManager")]
        public async Task<IActionResult> GetAllMartyrGraves(int managerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Lấy AccountId từ token
                var tokenAccountIdClaim = User.FindFirst("AccountId");
                if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
                {
                    return Forbid("Không tìm thấy AccountId trong token.");
                }

                var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

                // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
                if (tokenAccountId != managerId)
                {
                    return Forbid("Bạn không có quyền.");
                }

                // Sử dụng hàm mới để kiểm tra quyền của nhân viên hoặc quản lý
                var checkAuthorize = await _authorizeService.CheckAuthorizeManagerByAccountId(tokenAccountId, managerId);
                if (!checkAuthorize.isMatchedAccountManager || !checkAuthorize.isAuthorizedAccount)
                {
                    return Forbid();
                }
                // Gọi phương thức từ service để lấy danh sách mộ liệt sĩ có phân trang
                var graves = await _martyrGraveService.GetAllMartyrGravesForManagerAsync(page, pageSize, managerId);

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
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("update-grave-v2/{id}")]
        public async Task<IActionResult> UpdateMartyrGraveV2(int id, MartyrGraveUpdateDtoRequest martyrGraveDto)
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



        [HttpGet("area/{areaId}")]
        public async Task<IActionResult> GetMartyrGraveByAreaId(int areaId, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var result = await _martyrGraveService.GetMartyrGraveByAreaIdAsync(areaId, pageIndex, pageSize);
                return Ok(new { martyrGraves = result.martyrGraves, totalPage = result.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

    }
}
