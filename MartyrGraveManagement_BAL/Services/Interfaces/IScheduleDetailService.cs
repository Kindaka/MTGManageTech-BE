using MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IScheduleDetailService
    {
        Task<List<string>> CreateScheduleDetail(List<ScheduleDetailDtoRequest> requests, int accountId);
        Task<List<string>> CreateScheduleDetailForRecurringService(List<ScheduleDetailDtoRequest> requests, int accountId);
        //Task<string> UpdateScheduleDetail(int slotId, DateTime Date, int accountId, int Id);
        Task<string> DeleteScheduleDetail(int accountId, int Id);
        Task<List<ScheduleDetailListDtoResponse>> GetScheduleDetailStaff(int accountId, DateTime Date);
        Task<ScheduleDetailForTaskDtoResponse> GetScheduleDetailById(int accountId, int scheduleDetailId);
        Task<List<ScheduleDetailListDtoResponse>> GetSchedulesStaff(int accountId, DateTime FromDate, DateTime ToDate);
    }
}
