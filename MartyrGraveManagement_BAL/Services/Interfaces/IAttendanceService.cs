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
        Task<List<string>> CheckAttendances(List<CheckAttendancesDtoRequest> checkList, int managerId);
        Task<(bool status, string responseContent)> CheckAttendanceForStaff(CheckAttendanceForStaffDtoRequest request, int staffId);
        Task<string> UpdateAttendancStatus(int attendanceId, int status, string? Note, int managerId);
        Task<List<AttendanceDtoResponse>> GetAttendances(int managerId);
        Task<List<AttendanceDtoResponse>> GetAttendancesByStaffId(DateTime Date, int staffId);
        Task<AttendanceDtoResponse> GetAttendanceByAttendanceId(int attendanceId);
        Task<List<AttendanceDtoResponse>> GetAttendancesBySchedule(int slotId, DateTime Date, int managerId);
        //Task<AttendanceDtoResponse> GetAttendancesByScheduleForStaff(int slotId, DateTime Date, int staffId);
        Task<IEnumerable<AttendanceSlotDateDtoResponse>> GetAttendanceSlotDates(DateTime startDate, DateTime endDate, int managerId);
    }
}
