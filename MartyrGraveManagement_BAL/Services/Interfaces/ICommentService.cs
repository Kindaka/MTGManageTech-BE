using MartyrGraveManagement_BAL.ModelViews.CommentDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDTO>> GetAllCommentsAsync(int blogId);
        Task<string> CreateCommentAsync(CreateCommentDTO request, int accountId);
        Task<string> UpdateCommentAsync(int commentId, UpdateCommentDTO request);
        Task<string> DeleteCommentAsync(int commentId);
        Task<string> UpdateCommentStatusAsync(int commentId, bool status);
        Task<List<CommentDTO>> GetAllCommentsWithStatusTrueAsync(int blogId);
    }
}
