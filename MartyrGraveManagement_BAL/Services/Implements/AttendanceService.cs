using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<(bool status, string responseContent)> CheckAttendance(int attendanceId, int status)
        {
            try
            {
                if (status != 1 && status != 2) {
                    return (false, "Trạng thái không đúng, kiểm tra lại");
                }
                var attendance = await _unitOfWork.AttendanceRepository.GetByIDAsync(attendanceId);
                if (attendance != null) {
                    var schedule = await _unitOfWork.ScheduleRepository.GetByIDAsync(attendance.ScheduleId);
                    if (schedule.Date == DateOnly.FromDateTime(DateTime.Now))
                    {
                        attendance.Status = status; //1 là điểm danh có mặt, 2 là vắng mặt
                        attendance.UpdatedAt = DateTime.Now;
                        await _unitOfWork.AttendanceRepository.UpdateAsync(attendance);
                        return (true, "Đã cập nhật thành công");
                    }
                    else
                    {
                        return (false, "Thời gian điểm danh không trùng với lịch (phải tới ngày làm việc)");
                    }
                }
                else
                {
                    return (false, "Không tìm thấy staff để điểm danh");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AttendanceDtoResponse>> GetAttendances(int managerId)
        {
            try
            {
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                var attendanceList = new List<AttendanceDtoResponse>();
                if (manager != null) {
                    var attedances = await _unitOfWork.AttendanceRepository.GetAsync(includeProperties: "Schedule.Slot,Account");
                    if (attedances != null) {
                        foreach (var attendance in attedances)
                        {
                            if (attendance.Account.RoleId == 3 && attendance.Account.AreaId == manager.AreaId) {
                                var attendanceItem = new AttendanceDtoResponse
                                {
                                    staffName = attendance.Account.FullName,
                                    Date = attendance.Schedule.Date,
                                    StartTime = attendance.Schedule.Slot.StartTime,
                                    EndTime = attendance.Schedule.Slot.EndTime,
                                    status = attendance.Status,
                                };
                                attendanceList.Add(attendanceItem);
                            }
                        }
                        return attendanceList;
                    }
                    else
                    {
                        return attendanceList;
                    }
                }
                else
                {
                    return attendanceList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<AttendanceDtoResponse>> GetAttendancesByScheduleId(int scheduleId)
        {
            try
            {
                var attendanceList = new List<AttendanceDtoResponse>();
                var schedule = await _unitOfWork.ScheduleRepository.GetByIDAsync(scheduleId);

                if (schedule != null)
                {
                    var attedances = await _unitOfWork.AttendanceRepository.GetAsync(a => a.ScheduleId == scheduleId ,includeProperties: "Schedule.Slot,Account");
                    if (attedances != null)
                    {
                        foreach (var attendance in attedances)
                        {
                            var attendanceItem = new AttendanceDtoResponse
                            {
                                staffName = attendance.Account.FullName,
                                Date = attendance.Schedule.Date,
                                StartTime = attendance.Schedule.Slot.StartTime,
                                EndTime = attendance.Schedule.Slot.EndTime,
                                status = attendance.Status,
                            };
                            attendanceList.Add(attendanceItem);
                            
                        }
                        return attendanceList;
                    }
                    else
                    {
                        return attendanceList;
                    }
                }
                else
                {
                    return attendanceList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
