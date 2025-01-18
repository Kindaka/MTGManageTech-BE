using MartyrGraveManagement_BAL.ModelViews.DashboardDTOs;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IStatisticService
    {
        Task<DashboardDto> GetDashboard(int year);
        Task<DashboardDto> GetDashboardByAreaId(int year, int areaId);
        Task<WorkPerformanceStaff> GetWorkPerformanceStaff(int staffId, int managerId, int month, int year);
    }
}
