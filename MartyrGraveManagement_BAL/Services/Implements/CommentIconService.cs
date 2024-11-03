using MartyrGraveManagement_BAL.ModelViews.CommentIconDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class CommentIconService : ICommentIconService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentIconService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Comment_Icon> GetCommentIconByIdAsync(int id)
        {
            return await _unitOfWork.CommentIconRepository.GetByIDAsync(id);
        }

        public async Task<List<CreateCommentIconDTO>> GetAllCommentIconsAsync(int commentId)
        {
            var commentIcons = await _unitOfWork.CommentIconRepository.GetAsync(
                filter: ci => ci.CommentId == commentId,
                includeProperties: "Icon,Account"
            );

            return commentIcons.Select(ci => new CreateCommentIconDTO
            {
                Id = ci.Id,
                IconId = ci.IconId,
                CommentId = ci.CommentId,
                AccountId = ci.AccountId,
                CreateDate = ci.CreateDate,
                UpdateDate = ci.UpdateDate,
                Status = ci.Status,
                IconImage = ci.Icon?.IconImage,
                AccountName = ci.Account?.FullName
            }).ToList();
        }

        public async Task<string> CreateCommentIconAsync(int commentId, int iconId, int accountId)
        {
            var existingCommentIcon = await _unitOfWork.CommentIconRepository.FindAsync(
                ci => ci.CommentId == commentId && ci.AccountId == accountId
            );

            if (existingCommentIcon.Any())
            {
                return "Bạn đã thả biểu cảm cho bình luận này rồi.";
            }

            var newCommentIcon = new Comment_Icon
            {
                IconId = iconId,
                CommentId = commentId,
                AccountId = accountId,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Status = true
            };

            await _unitOfWork.CommentIconRepository.AddAsync(newCommentIcon);
            await _unitOfWork.SaveAsync();

            return "Comment icon created successfully.";
        }


        // Cập nhật CommentIcon
        public async Task<string> UpdateCommentIconAsync(int id, int iconId)
        {
            var commentIcon = await _unitOfWork.CommentIconRepository.GetByIDAsync(id);
            if (commentIcon == null)
            {
                throw new KeyNotFoundException("Comment icon not found.");
            }

            commentIcon.IconId = iconId;
            commentIcon.UpdateDate = DateTime.Now;
            await _unitOfWork.CommentIconRepository.UpdateAsync(commentIcon);
            await _unitOfWork.SaveAsync();

            return "Comment icon updated successfully.";
        }

        public async Task<string> DeleteCommentIconAsync(int id)
        {
            var commentIcon = await _unitOfWork.CommentIconRepository.GetByIDAsync(id);
            if (commentIcon == null)
            {
                throw new KeyNotFoundException("Comment icon not found.");
            }

            await _unitOfWork.CommentIconRepository.DeleteAsync(commentIcon);
            await _unitOfWork.SaveAsync();

            return "Comment icon deleted successfully.";
        }
    }

}
