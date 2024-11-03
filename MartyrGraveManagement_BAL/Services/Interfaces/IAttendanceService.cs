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
        Task<(bool status, string responseContent)> CheckAttendance(int attendanceId, int status);
        Task<List<AttendanceDtoResponse>> GetAttendances(int managerId);
        Task<List<AttendanceDtoResponse>> GetAttendancesByScheduleId(int scheduleId);
    }
}
