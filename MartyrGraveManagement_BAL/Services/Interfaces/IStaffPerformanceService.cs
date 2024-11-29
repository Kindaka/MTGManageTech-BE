using MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs;
using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IStaffPerformanceService
    {
        Task<WorkPerformanceDTO> EvaluatePerformance(int staffId, DateTime startDate, DateTime endDate);

        // Lấy lịch sử đánh giá của một nhân viên
        Task<(IEnumerable<WorkPerformanceDTO> performances, int totalPage)> GetStaffPerformanceHistory(
        int staffId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 10);


        // Tạo báo cáo hiệu suất dạng Excel
        
        Task<Stream> GenerateStaffPerformanceReport(int workId);
        Task<WorkPerformanceDTO> UpdatePerformance(int workId, UpdatePerformanceDTO updateDTO);

        Task<bool> DeletePerformance(int workId);

        Task<WorkPerformanceDTO> GetPerformanceById(int workId);

    }
}
