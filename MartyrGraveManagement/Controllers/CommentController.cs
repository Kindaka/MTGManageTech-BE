using MartyrGraveManagement_BAL.ModelViews.CommentDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IAuthorizeService _authorizeService;
        public CommentController(ICommentService commentService, IAuthorizeService authorizeService)
        {
            _commentService = commentService;
            _authorizeService = authorizeService;
        }

        /// <summary>
        /// Get All Comment (manager or staff)
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpGet("{blogId}")]
        public async Task<IActionResult> GetAllComments(int blogId)
        {
            var comments = await _commentService.GetAllCommentsAsync(blogId);
            return Ok(new { message = "Comments retrieved successfully.", data = comments });
        }


        /// <summary>
        /// Get All Comment with status true (customer)
        /// </summary>
        [HttpGet("blog/{blogId}/comments")]
        public async Task<IActionResult> GetAllCommentsWithStatusTrue(int blogId)
        {
            var comments = await _commentService.GetAllCommentsWithStatusTrueAsync(blogId);
            return Ok(new { message = "Comments retrieved successfully.", data = comments });
        }


        /// <summary>
        /// Create Comment (Customer)
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO request)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            int tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(tokenAccountId, tokenAccountId);
            if (!checkAuthorize.isMatchedAccount || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }

            // Gọi service và truyền accountId từ token
            var result = await _commentService.CreateCommentAsync(request, tokenAccountId);
            return Ok(new { message = result });
        }

        /// <summary>
        /// Update Comment (Customer)
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentDTO request, int accountId)
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

            var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccount || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }
            var result = await _commentService.UpdateCommentAsync(commentId, request);
            return Ok(new { message = result });
        }
        /// <summary>
        /// Update Status (Staff or Manager)
        /// </summary>
        [Authorize(Policy = "RequireManagerOrStaffRole")]
        [HttpPut("status/{commentId}/{status}")]
        public async Task<IActionResult> UpdateCommentStatus(int commentId, bool status)
        {
            var result = await _commentService.UpdateCommentStatusAsync(commentId, status);
            return Ok(new { message = result });
        }

        /// <summary>
        /// Delete Comment (Customer)
        /// </summary>
        [Authorize(Policy = "RequireCustomerRole")]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId, int accountId)
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

            var checkAuthorize = await _authorizeService.CheckAuthorizeByAccountId(tokenAccountId, accountId);
            if (!checkAuthorize.isMatchedAccount || !checkAuthorize.isAuthorizedAccount)
            {
                return Forbid();
            }
            var result = await _commentService.DeleteCommentAsync(commentId);
            return Ok(new { message = result });
        }
    }
}
