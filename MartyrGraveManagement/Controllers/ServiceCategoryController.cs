using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly IServiceCategory_Service _categoryService;
        public ServiceCategoryController(IServiceCategory_Service categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get all service categories.
        /// </summary>
        /// <returns>Returns a list of all service categories.</returns>
        /// <response code="200">Returns the list of categories</response>
        /// <response code="500">If there is any server error</response>
        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var categories = await _categoryService.GetAllServiceCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add a new service category.
        /// </summary>
        /// <param name="category">Details of the new service category.</param>
        /// <returns>Returns success or failure message.</returns>
        /// <response code="200">If the category is created successfully</response>
        /// <response code="400">If the category creation fails</response>
        /// <response code="500">If there is any server error</response>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory(ServiceCategoryDto category)
        {
            try
            {
                var check = await _categoryService.AddServiceCategory(category);
                if (check)
                {
                    return Ok("Create successfully");
                }
                else
                {
                    return BadRequest("Create unsuccessfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing service category.
        /// </summary>
        /// <param name="category">Updated details of the service category.</param>
        /// <param name="id">ID of the service category to update.</param>
        /// <returns>Returns the result of the update operation.</returns>
        /// <response code="200">If the category is updated successfully</response>
        /// <response code="400">If the update fails</response>
        /// <response code="500">If there is any server error</response>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("categories")]
        public async Task<IActionResult> UpdateCategory(ServiceCategoryDto category, int id)
        {
            try
            {
                var check = await _categoryService.UpdateServiceCategory(category, id);
                if (check.status)
                {
                    return Ok(check.result);
                }
                else
                {
                    return BadRequest(check.result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
