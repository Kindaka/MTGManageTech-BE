using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("import-locations")]
        public async Task<IActionResult> ImportLocations(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.GetTempFileName(); // Temporary file path for processing

            try
            {
                // Save the file temporarily
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Import locations from the Excel file
                var (status, message) = await _locationService.ImportLocationsFromExcelAsync(filePath);

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
    }
}
