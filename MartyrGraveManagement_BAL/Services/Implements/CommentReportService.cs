using MartyrGraveManagement_BAL.ModelViews.CommentDTOs;
using MartyrGraveManagement_BAL.ModelViews.CommentReportDTOs;
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
    public class CommentReportService : ICommentReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CommentReportDTO>> GetAllCommentReportsAsync()
        {
            var reports = await _unitOfWork.CommentReportRepository.GetAsync(includeProperties: "Account,Comment");
            return reports.Select(r => new CommentReportDTO
            {
                ReportId = r.ReportId,
                AccountId = r.AccountId,
                AccountName = r.Account?.FullName,
                CommentId = r.CommentId,
                Title = r.Title,
                Content = r.Content,
                CreatedDate = r.CreatedDate,
                UpdatedDate = r.UpdatedDate,
                Status = r.Status
            }).ToList();
        }

        public async Task<CommentReportDTO> GetCommentReportByIdAsync(int reportId)
        {
            var reports = await _unitOfWork.CommentReportRepository.GetAsync(
                filter: r => r.ReportId == reportId,
                includeProperties: "Account"
            );
            var report = reports.FirstOrDefault();

            if (report == null) throw new KeyNotFoundException("Report not found.");

            return new CommentReportDTO
            {
                ReportId = report.ReportId,
                AccountId = report.AccountId,
                AccountName = report.Account?.FullName,
                AccountAvatar = report.Account?.AvatarPath,

                CommentId = report.CommentId,
                Title = report.Title,
                Content = report.Content,
                CreatedDate = report.CreatedDate,
                UpdatedDate = report.UpdatedDate,
                Status = report.Status
            };
        }

        public async Task<string> DeleteCommentReportAsync(int reportId)
        {
            var comment = await _unitOfWork.CommentReportRepository.GetByIDAsync(reportId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");

            comment.Status = false;
            comment.UpdatedDate = DateTime.Now;

            await _unitOfWork.CommentReportRepository.UpdateAsync(comment);
            await _unitOfWork.SaveAsync();

            return "Comment updated successfully.";
        }


        public async Task<string> CreateCommentReportAsync(int commentId, CreateCommentReportDTO request, int accountId)
        {
            var newReport = new Comment_Report
            {
                AccountId = accountId,
                CommentId = commentId,
                Title = request.Title,
                Content = request.Content,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                Status = true
            };

            await _unitOfWork.CommentReportRepository.AddAsync(newReport);
            await _unitOfWork.SaveAsync();

            return "Comment report created successfully.";
        }
    }
}
