using MartyrGraveManagement_BAL.ModelViews.BlogCategoryDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCategoryController : Controller
    {
        private readonly IBlogCategoryService _blogCategoryService;

        public BlogCategoryController(IBlogCategoryService blogCategoryService)
        {
            _blogCategoryService = blogCategoryService;
        }


        /// <summary>
        /// Get all BlogCategory True/False (Manager)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _blogCategoryService.GetAllBlogCategoriesAsync();
            return Ok(result);
        }

        /// <summary>
        /// GetById
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _blogCategoryService.GetBlogCategoryByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Blog category not found." });
            }
            return Ok(result);
        }

        /// <summary>
        /// Get all BlogCategories with status true and pagination
        /// </summary>
        [AllowAnonymous]
        [HttpGet("GetAllBlogCategoriesWithStatusTrue")]
        public async Task<IActionResult> GetAllBlogCategoriesByStatusTrueAsync(int pageIndex = 1, int pageSize = 5)
        {
            try
            {
                var (blogCategories, totalPage) = await _blogCategoryService.GetAllBlogCategoriesByStatusTrueAsync(pageIndex, pageSize);
                return Ok(new { message = "Blog Categories retrieved successfully.", data = blogCategories, totalPage = totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi lấy danh sách Blog Categories: {ex.Message}" });
            }
        }


        /// <summary>
        /// Create a new BlogCategory (Manager)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlogCategoryDtoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (status, message) = await _blogCategoryService.CreateBlogCategoryAsync(request);
            if (!status)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Blog category created successfully." });
        }


        /// <summary>
        ///  Update BlogCategory (Manager)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BlogCategoryDtoRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (status, message) = await _blogCategoryService.UpdateBlogCategoryAsync(id, request);
            if (!status)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Blog category updated successfully." });
        }

        /// <summary>
        /// Delete a BlogCategory (Manager)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (status, message) = await _blogCategoryService.DeleteBlogCategoryAsync(id);
            if (!status)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Blog category deleted successfully." });
        }


        /// <summary>
        /// Change status (Manager)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("update-status/{historyId}/{newStatus}")]
        public async Task<IActionResult> UpdateBlogCategoryStatus(int historyId, bool newStatus)
        {
            try
            {
                var result = await _blogCategoryService.UpdateBlogCategoryStatusAsync(historyId, newStatus);

                if (!result)
                {
                    return NotFound(new { message = "BlogCategory không tồn tại hoặc không thể cập nhật." });
                }

                return Ok(new { message = "Cập nhật trạng thái BlogCategory thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi xảy ra khi cập nhật trạng thái: {ex.Message}" });
            }
        }

    }
}
