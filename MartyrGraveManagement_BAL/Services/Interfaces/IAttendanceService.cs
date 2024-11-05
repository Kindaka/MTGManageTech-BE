using MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<List<string>> CheckAttendance(List<CheckAttendancesDtoRequest> checkList);
        Task<List<AttendanceDtoResponse>> GetAttendances(int managerId);
        Task<List<AttendanceDtoResponse>> GetAttendancesByScheduleId(int slotId, DateTime Date);
    }
}
