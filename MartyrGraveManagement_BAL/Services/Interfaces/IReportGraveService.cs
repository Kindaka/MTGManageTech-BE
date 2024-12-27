using MartyrGraveManagement_BAL.ModelViews.ReportGraveDTOs;
using Microsoft.AspNetCore.Http;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IReportGraveService
    {
        Task<bool> UploadVideoAsync(IFormFile file, int staffId, int reportId);
        Task<ReportGraveDtoResponse> GetReportGraveById(int id);
        Task<ReportGraveDtoResponse> GetReportGraveByRequestId(int requestId);
        Task<(IEnumerable<ReportGraveDtoResponse> reportList, int totalPage)> GetReportsForStaff(
            int staffId,
            int pageIndex,
            int pageSize,
            DateTime Date);
    }
}
