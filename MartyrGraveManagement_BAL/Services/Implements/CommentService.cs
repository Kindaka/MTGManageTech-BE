using MartyrGraveManagement_BAL.ModelViews.CommentDTOs;
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
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CommentDTO>> GetAllCommentsAsync(int blogId)
        {
            var comments = await _unitOfWork.CommentRepository.GetAsync(
                filter: c => c.BlogId == blogId,
                includeProperties: "Account,Comment_Icons.Icon"
            );

            return comments.Select(comment => new CommentDTO
            {
                CommentId = comment.CommentId,
                AccountId = comment.AccountId,
                AccountName = comment.Account?.FullName,
                BlogId = comment.BlogId,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate,
                UpdatedDate = comment.UpdatedDate,
                Status = comment.Status,
                CommentIcons = comment.Comment_Icons
                    .GroupBy(icon => icon.IconId)
                    .Select(group => new CommentIconDTO
                    {
                        Id = group.First().Id,
                        IconId = group.Key,
                        IconImage = group.First().Icon?.IconImage,
                        Count = group.Count()
                    }).ToList()
            }).ToList();
        }

        public async Task<List<CommentDTO>> GetAllCommentsWithStatusTrueAsync(int blogId)
        {
            var comments = await _unitOfWork.CommentRepository.GetAsync(
                filter: c => c.BlogId == blogId && c.Status == true,
                includeProperties: "Account,Comment_Icons.Icon"
            );

            return comments.Select(comment => new CommentDTO
            {
                CommentId = comment.CommentId,
                AccountId = comment.AccountId,
                AccountName = comment.Account?.FullName,
                BlogId = comment.BlogId,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate,
                UpdatedDate = comment.UpdatedDate,
                Status = comment.Status,
                CommentIcons = comment.Comment_Icons
                    .GroupBy(icon => icon.IconId)
                    .Select(group => new CommentIconDTO
                    {
                        Id = group.First().Id,
                        IconId = group.Key,
                        IconImage = group.First().Icon?.IconImage,
                        Count = group.Count()
                    }).ToList()
            }).ToList();
        }



        public async Task<string> CreateCommentAsync(CreateCommentDTO request, int accountId)
        {
            var newComment = new Comment
            {
                AccountId = accountId,
                BlogId = request.BlogId,
                Content = request.Content,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Status = true
            };

            await _unitOfWork.CommentRepository.AddAsync(newComment);
            await _unitOfWork.SaveAsync();

            return "Comment created successfully.";
        }


        public async Task<string> UpdateCommentAsync(int commentId, UpdateCommentDTO request)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIDAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");

            comment.Content = request.Content;
            comment.UpdatedDate = DateTime.Now;

            await _unitOfWork.CommentRepository.UpdateAsync(comment);
            await _unitOfWork.SaveAsync();

            return "Comment updated successfully.";
        }


        public async Task<string> UpdateCommentStatusAsync(int commentId, bool status)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIDAsync(commentId);
            if (comment == null)
            {
                return "Comment not found.";
            }

            comment.Status = status;

            await _unitOfWork.CommentRepository.UpdateAsync(comment);
            await _unitOfWork.SaveAsync();

            return "Comment status updated successfully.";
        }



        public async Task<string> DeleteCommentAsync(int commentId)
        {
            // Lấy thông tin comment dựa vào commentId
            var comment = await _unitOfWork.CommentRepository.GetByIDAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");

            // Xóa tất cả CommentIcons liên quan đến comment
            var commentIcons = await _unitOfWork.CommentIconRepository.FindAsync(ci => ci.CommentId == commentId);
            await _unitOfWork.CommentIconRepository.DeleteRangeAsync(commentIcons);

            // Xóa comment
            await _unitOfWork.CommentRepository.DeleteAsync(comment);
            await _unitOfWork.SaveAsync();

            return "Comment and associated icons deleted successfully.";
        }

    }
}
