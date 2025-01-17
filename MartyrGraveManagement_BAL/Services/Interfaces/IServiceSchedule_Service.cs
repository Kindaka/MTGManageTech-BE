using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IServiceSchedule_Service
    {
        Task<(bool status, string response)> CreateServiceSchedule(ServiceScheduleDtoRequest request);
        Task<List<ServiceScheduleDtoResponse>> GetServiceScheduleByAccountId(int accountId);
        Task<ServiceScheduleDetailResponse> GetServiceScheduleById(int serviceScheduleId);
        Task<bool> UpdateStatusServiceSchedule(int serviceScheduleId, int customerId);
    }
}
