using MartyrGraveManagement_BAL.ModelViews.CommentIconDTOs;
using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace MartyrGraveManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentIconController : ControllerBase
    {
        private readonly ICommentIconService _commentIconService;
        private readonly IAuthorizeService _authorizeService;

        public CommentIconController(ICommentIconService commentIconService, IAuthorizeService authorizeService)
        {
            _commentIconService = commentIconService;
            _authorizeService = authorizeService;
        }

        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetAllCommentIcons(int commentId)
        {
            var result = await _commentIconService.GetAllCommentIconsAsync(commentId);
            return Ok(result);
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<IActionResult> CreateCommentIcon(int commentId, int iconId)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            int accountId = int.Parse(tokenAccountIdClaim.Value);

            try
            {
                var result = await _commentIconService.CreateCommentIconAsync(commentId, iconId, accountId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi tạo Comment icon: {ex.Message}" });
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommentIcon(int id, [FromBody] UpdateCommentIconDTO request)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra xem CommentIcon có tồn tại và thuộc về tài khoản không
            var commentIcon = await _commentIconService.GetCommentIconByIdAsync(id);
            if (commentIcon == null)
            {
                return NotFound("Comment icon không tồn tại.");
            }

            // Kiểm tra nếu AccountId từ JWT có khớp với AccountId của CommentIcon không
            if (commentIcon.AccountId != tokenAccountId)
            {
                return Forbid("Bạn không có quyền cập nhật icon của comment này.");
            }

            var result = await _commentIconService.UpdateCommentIconAsync(id, request.IconId);
            return Ok(new { message = result });
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentIcon(int id)
        {
            // Lấy AccountId từ token
            var tokenAccountIdClaim = User.FindFirst("AccountId");
            if (tokenAccountIdClaim == null || string.IsNullOrEmpty(tokenAccountIdClaim.Value))
            {
                return Forbid("Không tìm thấy AccountId trong token.");
            }

            var tokenAccountId = int.Parse(tokenAccountIdClaim.Value);

            // Kiểm tra xem CommentIcon có tồn tại và thuộc về tài khoản không
            var commentIcon = await _commentIconService.GetCommentIconByIdAsync(id);
            if (commentIcon == null)
            {
                return NotFound("Comment icon không tồn tại.");
            }

            // Kiểm tra nếu AccountId từ JWT có khớp với AccountId của CommentIcon không
            if (commentIcon.AccountId != tokenAccountId)
            {
                return Forbid("Bạn không có quyền xóa icon của comment này.");
            }

            var result = await _commentIconService.DeleteCommentIconAsync(id);
            return Ok(new { message = result });
        }


    }

}
