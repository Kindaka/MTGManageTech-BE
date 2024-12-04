using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IServiceSchedule_Service
    {
        Task<(bool status, string response)> CreateServiceSchedule(ServiceScheduleDtoRequest request);
        Task<List<ServiceScheduleDtoResponse>> GetServiceScheduleByAccountId(int accountId);
        Task<ServiceScheduleDetailResponse> GetServiceScheduleById(int serviceScheduleId);
    }
}
