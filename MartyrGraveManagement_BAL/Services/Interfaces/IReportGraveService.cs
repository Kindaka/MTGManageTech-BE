using MartyrGraveManagement_BAL.ModelViews.ReportGraveDTOs;
using Microsoft.AspNetCore.Http;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IReportGraveService
    {
        Task<bool> UploadVideoAsync(IFormFile file, int staffId, int reportId);
        Task<ReportGraveDtoResponse> GetReportGraveById(int id);
    }
}
