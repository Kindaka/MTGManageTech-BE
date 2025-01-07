using MartyrGraveManagement_BAL.ModelViews.BlogDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IAuthorizeService _authorizeService;

        public BlogController(IBlogService blogService, IAuthorizeService authorizeService)
        {
            _blogService = blogService;
            _authorizeService = authorizeService;
        }

        /// <summary>
        /// View Blog By  AccountId (Staff)
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("GetBlogByAccountId/{accountId}")]
        public async Task<IActionResult> GetBlogByAccountId(int accountId)
        {

            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != accountId)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
            }

            var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }
            try
            {
                var blogs = await _blogService.GetBlogByAccountId(accountId);
                if (blogs == null || !blogs.Any())
                {
                    return NotFound(new { message = "No blogs found for this account." });
                }
                return Ok(new { message = "Blogs retrieved successfully.", data = blogs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Create Blog (accountId take from jwt role staff)
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPost("CreateBlog")]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDTORequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Yêu cầu không hợp lệ." });
            }

            // Lấy AccountId từ token
            var accountIdClaim = User.FindFirst("AccountId");
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            int accountId = int.Parse(accountIdClaim.Value);

            try
            {
                var result = await _blogService.CreateBlogAsync(request, accountId);

                if (result == "Blog created successfully.")
                {
                    return Ok(new { message = result });
                }
                else
                {
                    return BadRequest(new { message = result });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tạo Blog: {ex.Message}" });
            }
        }

        ///// <summary>
        ///// View All Blog (True and False) Manager
        ///// </summary>
        //[Authorize(Policy = "RequireManagerRole")]
        //[HttpGet("GetAllBlogs")]
        //public async Task<IActionResult> GetAllBlogs()
        //{
        //    try
        //    {
        //        var blogs = await _blogService.GetAllBlogsAsync();
        //        return Ok(new { message = "Blogs retrieved successfully.", data = blogs });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = $"Lỗi khi lấy danh sách Blogs: {ex.Message}" });
        //    }
        //}

        /// <summary>
        /// View All Blogs with status true and pagination (Customer)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("GetAllBlogsForCustomer")]
        public async Task<IActionResult> GetAllBlogsWithStatusTrueAsync(int pageIndex = 1, int pageSize = 5)
        {
            try
            {
                var (blogs, totalPage) = await _blogService.GetAllBlogsWithStatusTrueAsync(pageIndex, pageSize);
                return Ok(new { message = "Blogs retrieved successfully.", data = blogs, totalPage = totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi lấy danh sách Blogs: {ex.Message}" });
            }
        }


        /// <summary>
        /// View Detail Blog 
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [AllowAnonymous]
        [HttpGet("GetBlogById/{blogId}")]
        public async Task<IActionResult> GetBlogById(int blogId)
        {
            try
            {
                var blog = await _blogService.GetBlogByIdAsync(blogId);
                if (blog == null)
                {
                    return NotFound(new { message = "Blog không tồn tại." });
                }
                return Ok(new { message = "Blog retrieved successfully.", data = blog });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi lấy chi tiết Blog: {ex.Message}" });
            }
        }

        /// <summary>
        /// Update Blog 
        /// </summary>
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("UpdateBlog/{blogId}")]
        public async Task<IActionResult> UpdateBlog(int blogId, [FromBody] CreateBlogDTORequest request, int accountId)
        {

            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra nếu AccountId trong URL có khớp với AccountId trong token không
            if (tokenAccountId != accountId)
            {
                return Forbid("Bạn không có quyền cập nhật thông tin của tài khoản này.");
            }

            var checkAuthorize = await _authorizeService.CheckAuthorizeStaffByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccountStaff || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }
            var result = await _blogService.UpdateBlogAsync(blogId, request);
            if (result == "Cập nhật Blog thành công.")
            {
                return Ok(new { message = result });
            }
            return BadRequest(new { message = result });
        }

        /// <summary>
        /// Update Blog Status (Manager)
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPatch("UpdateBlogStatus/{blogId}")]
        public async Task<IActionResult> UpdateBlogStatus(int blogId, [FromQuery] bool status)
        {
            var result = await _blogService.UpdateBlogStatusAsync(blogId, status);
            if (result == "Cập nhật trạng thái Blog thành công.")
            {
                return Ok(new { message = result });
            }
            return BadRequest(new { message = result });
        }


        /// <summary>
        /// Get blogs for manager.
        /// </summary>
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet("blogs/manager/{managerId}")]
        public async Task<IActionResult> GetBlogsBymanagerId(int managerId, DateTime Date, int pageIndex = 1, int pageSize = 5)
        {//
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeManagerByAccountId(managerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedAccountManager)
                {
                    return Forbid();
                }

                var blogs = await _blogService.GetAllBlogsAsync(managerId, pageIndex, pageSize, Date);


                return Ok(new { blogs = blogs.blogList, totalPage = blogs.totalPage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
