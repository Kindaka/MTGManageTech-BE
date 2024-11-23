using MartyrGraveManagement_BAL.ModelViews.CommentReportDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ICommentReportService
    {
        Task<List<CommentReportDTO>> GetAllCommentReportsAsync();
        Task<CommentReportDTO> GetCommentReportByIdAsync(int reportId);
        Task<string> CreateCommentReportAsync(int commentId, CreateCommentReportDTO request, int accountId);

        Task<string> DeleteCommentReportAsync(int reportId);
    }
}
