using MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleDTOResponse>> GetScheduleByAccountId(int accountId);

        Task<string> UpdateSchedule(int scheduleId, UpdateScheduleDTORequest request);
        Task<ScheduleDTOResponse> GetScheduleById(int scheduleId);
        Task<string> DeleteSchedule(int scheduleId);

        Task<List<string>> CreateSchedule(List<CreateScheduleDTORequest> requests);
    }
}
