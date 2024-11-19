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

        public async Task<(bool status, string responseContent)> CheckAttendanceForStaff(CheckAttendanceForStaffDtoRequest request, int staffId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var checkAttendance = (await _unitOfWork.AttendanceRepository.GetAsync(a => a.AttendanceId == request.AttendanceId, includeProperties: "Slot,Account")).FirstOrDefault();
                    if (checkAttendance != null)
                    {
                        if (checkAttendance.AccountId != staffId)
                        {
                            return (false, "Điểm danh không phải của bạn");
                        }
                        if (checkAttendance.Date == DateOnly.FromDateTime(DateTime.Now) && checkAttendance.Slot.StartTime <= TimeOnly.FromDateTime(DateTime.Now) && checkAttendance.Slot.EndTime >= TimeOnly.FromDateTime(DateTime.Now))
                        {
                            checkAttendance.ImagePath1 = request.ImagePath1;
                            if (request.ImagePath2 != null && request.ImagePath3 != null)
                            {
                                checkAttendance.ImagePath2 = request.ImagePath2;
                                checkAttendance.ImagePath3 = request.ImagePath3;
                            }
                            checkAttendance.Status = 1;
                            await _unitOfWork.AttendanceRepository.UpdateAsync(checkAttendance);
                            await transaction.CommitAsync();
                            return (true, "Đã cập nhật điểm danh thành công");
                        }
                        else
                        {
                            return (false, $"Thời gian điểm danh không trùng với lịch làm việc ngày {checkAttendance.Date} từ {checkAttendance.Slot.StartTime} đến {checkAttendance.Slot.EndTime}");
                        }
                    }
                    else
                    {
                        return (false, "Không tìm thấy điểm danh (Sai Id)");
                    }
                    
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<string>> CheckAttendances(List<CheckAttendancesDtoRequest> checkList, int managerId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var results = new List<string>();
                    var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                    if (manager == null)
                    {
                        results.Add("Không tìm thấy manager");
                        return results;
                    }
                    foreach (var checkAttendance in checkList)
                    {
                        if (checkAttendance.statusAttendance != 1 && checkAttendance.statusAttendance != 2)
                        {
                            //return (false, "Trạng thái không đúng, kiểm tra lại");
                            results.Add("Trạng thái không đúng, kiểm tra lại");
                            continue;
                        }
                        var attendance = (await _unitOfWork.AttendanceRepository.GetAsync(a => a.AttendanceId == checkAttendance.attendanceId, includeProperties: "Slot,Account")).FirstOrDefault();
                        if (attendance != null)
                        {
                            if (attendance.Account.RoleId == 3 && attendance.Account.AreaId == manager.AreaId)
                            {
                                if (attendance.Date == DateOnly.FromDateTime(DateTime.Now) && attendance.Slot.StartTime <= TimeOnly.FromDateTime(DateTime.Now) && attendance.Slot.EndTime >= TimeOnly.FromDateTime(DateTime.Now))
                                {
                                    attendance.Status = checkAttendance.statusAttendance; //1 là điểm danh có mặt, 2 là vắng mặt
                                    attendance.UpdatedAt = DateTime.Now;
                                    await _unitOfWork.AttendanceRepository.UpdateAsync(attendance);
                                }
                                else
                                {
                                    results.Add($"Thời gian điểm danh không trùng với lịch làm việc ngày {attendance.Date} từ {attendance.Slot.StartTime} đến {attendance.Slot.EndTime}");
                                    continue;
                                }
                            }
                            else
                            {
                                results.Add("Manager không cùng khu vực với staff");
                                continue;
                            }
                        }
                        else
                        {
                            results.Add("Không tìm thấy mục điểm danh (attendanceId không tìm thấy)");
                            continue;
                        }
                        results.Add("Đã cập nhật thành công");
                    }
                    await transaction.CommitAsync();
                    return results;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<AttendanceDtoResponse> GetAttendanceByAttendanceId(int attendanceId)
        {
            try
            {
                var attendance = (await _unitOfWork.AttendanceRepository.GetAsync(a => a.AttendanceId == attendanceId, includeProperties: "Slot,Account")).FirstOrDefault();
                if (attendance != null) {
                    var attendanceItem = new AttendanceDtoResponse
                    {
                        AttendanceId = attendance.AttendanceId,
                        AccountId = attendance.AccountId,
                        SlotId = attendance.SlotId,
                        staffName = attendance.Account.FullName,
                        Date = attendance.Date,
                        StartTime = attendance.Slot.StartTime,
                        EndTime = attendance.Slot.EndTime,
                        ImagePath1 = attendance.ImagePath1,
                        ImagePath2 = attendance.ImagePath2,
                        ImagePath3 = attendance.ImagePath3,
                        Note = attendance.Note,
                        status = attendance.Status,
                    };
                    return attendanceItem;
                }
                else
                {
                    return null;
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
                    var attedances = await _unitOfWork.AttendanceRepository.GetAsync(includeProperties: "Slot,Account");
                    if (attedances != null) {
                        foreach (var attendance in attedances)
                        {
                            if (attendance.Account.RoleId == 3 && attendance.Account.AreaId == manager.AreaId) {
                                var attendanceItem = new AttendanceDtoResponse
                                {
                                    AttendanceId = attendance.AttendanceId,
                                    AccountId = attendance.AccountId,
                                    SlotId = attendance.SlotId,
                                    staffName = attendance.Account.FullName,
                                    Date = attendance.Date,
                                    StartTime = attendance.Slot.StartTime,
                                    EndTime = attendance.Slot.EndTime,
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

        public async Task<List<AttendanceDtoResponse>> GetAttendancesBySchedule(int slotId, DateTime Date, int managerId)
        {
            try
            {
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                var attendanceList = new List<AttendanceDtoResponse>();
                var slot = await _unitOfWork.SlotRepository.GetByIDAsync(slotId);

                if (slot != null && manager != null)
                {
                    var attedances = await _unitOfWork.AttendanceRepository.GetAsync(a => a.SlotId == slotId && a.Date == DateOnly.FromDateTime(Date),includeProperties: "Slot,Account");
                    if (attedances != null)
                    {
                        foreach (var attendance in attedances)
                        {
                            if (attendance.Account.RoleId == 3 && attendance.Account.AreaId == manager.AreaId)
                            {
                                var attendanceItem = new AttendanceDtoResponse
                                {
                                    AttendanceId = attendance.AttendanceId,
                                    AccountId = attendance.AccountId,
                                    PhoneNumber = attendance.Account.PhoneNumber,
                                    Email = attendance.Account.EmailAddress,
                                    SlotId = attendance.SlotId,
                                    staffName = attendance.Account.FullName,
                                    Date = attendance.Date,
                                    StartTime = attendance.Slot.StartTime,
                                    EndTime = attendance.Slot.EndTime,
                                    ImagePath1 = attendance.ImagePath1,
                                    ImagePath2 = attendance.ImagePath2,
                                    ImagePath3 = attendance.ImagePath3,
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

        //public Task<AttendanceDtoResponse> GetAttendancesByScheduleForStaff(int slotId, DateTime Date, int staffId)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<List<AttendanceDtoResponse>> GetAttendancesByStaffId(DateTime Date, int staffId)
        {
            try
            {
                var staff = await _unitOfWork.AccountRepository.GetByIDAsync(staffId);
                var attendanceList = new List<AttendanceDtoResponse>();
                if (staff != null)
                {
                    var attedances = await _unitOfWork.AttendanceRepository.GetAsync(a => a.AccountId == staffId && a.Date == DateOnly.FromDateTime(Date), includeProperties: "Slot,Account");
                    if (attedances != null)
                    {
                        foreach (var attendance in attedances)
                        {

                            var attendanceItem = new AttendanceDtoResponse
                            {
                                AttendanceId = attendance.AttendanceId,
                                PhoneNumber = attendance.Account.PhoneNumber,
                                Email = attendance.Account.EmailAddress,
                                AccountId = attendance.AccountId,
                                SlotId = attendance.SlotId,
                                staffName = attendance.Account.FullName,
                                Date = attendance.Date,
                                StartTime = attendance.Slot.StartTime,
                                EndTime = attendance.Slot.EndTime,
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

        public async Task<IEnumerable<AttendanceSlotDateDtoResponse>> GetAttendanceSlotDates(DateTime startDate, DateTime endDate, int managerId)
        {
            try
            {
                var groupedAttendances = new List<AttendanceSlotDateDtoResponse>();
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                var attendances = await _unitOfWork.AttendanceRepository.GetAsync(a => a.Date <= DateOnly.FromDateTime(endDate) && a.Date >= DateOnly.FromDateTime(startDate), includeProperties:"Account,Slot");
                if(attendances == null)
                {
                    return groupedAttendances;
                }
                foreach (var attendance in attendances) { 
                    if(attendance.Account.AreaId == manager.AreaId)
                    {
                        // Check if this Date and Slot combination already exists
                        bool exists = groupedAttendances.Any(g =>
                            g.Date == attendance.Date &&
                            g.SlotId == attendance.SlotId);

                        // Only add if the combination doesn't exist
                        if (!exists)
                        {
                            var groupedAttendance = new AttendanceSlotDateDtoResponse
                            {
                                SlotId = attendance.SlotId,
                                Date = attendance.Date,
                                SlotName = attendance.Slot.SlotName,
                                StartTime = attendance.Slot.StartTime,
                                EndTime = attendance.Slot.EndTime
                            };
                            groupedAttendances.Add(groupedAttendance);
                        }
                    }
                }
                return groupedAttendances;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting attendance slots and dates: {ex.Message}");
            }
        }

        public async Task<string> UpdateAttendancStatus(int attendanceId, int status, string? Note, int managerId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (status != 1 && status != 2)
                    {
                        return "Sai status của attendance (Phải là 1: đã điểm danh hoặc 2: vắng mặt";
                    }
                    var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                    if (manager == null)
                    {
                        return "Không tìm thấy manager";
                    }
                    var attendance = (await _unitOfWork.AttendanceRepository.GetAsync(a => a.AttendanceId == attendanceId, includeProperties: "Slot,Account")).FirstOrDefault();
                    if (attendance != null)
                    {
                        if (attendance.Account.RoleId == 3 && attendance.Account.AreaId == manager.AreaId)
                        {
                            if (attendance.Date == DateOnly.FromDateTime(DateTime.Now) && attendance.Slot.StartTime <= TimeOnly.FromDateTime(DateTime.Now))
                            {
                                attendance.Status = status;
                                attendance.UpdatedAt = DateTime.Now;
                                attendance.Note = Note;
                                await _unitOfWork.AttendanceRepository.UpdateAsync(attendance);
                                await transaction.CommitAsync();
                                return "Đã cập nhật điểm danh thành công";
                            }
                            else
                            {
                                return $"Thời gian điểm danh không trùng với lịch làm việc ngày {attendance.Date} từ {attendance.Slot.StartTime} đến {attendance.Slot.EndTime}";
                            }
                        }
                        else
                        {
                            return "Manager không cùng khu vực với staff";
                        }
                    }
                    else
                    {
                        return "Không tìm thấy attendance";
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
