using MartyrGraveManagement_BAL.ModelViews.CommentIconDTOs;
using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ICommentIconService
    {
        Task<string> DeleteCommentIconAsync(int id);
        Task<string> UpdateCommentIconAsync(int id, int iconId);
        Task<string> CreateCommentIconAsync(int commentId, int iconId, int accountId);
        Task<List<CreateCommentIconDTO>> GetAllCommentIconsAsync(int commentId);
        Task<Comment_Icon> GetCommentIconByIdAsync(int id);
    }
}
