using MartyrGraveManagement_BAL.ModelViews.ServiceCategoryDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory(ServiceCategoryDto category)
        {
            try
            {
                var check = await _categoryService.AddServiceCategory(category);
                if(check)
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
                throw new Exception(ex.Message);
            }
        }

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
                throw new Exception(ex.Message);
            }
        }
    }
}
