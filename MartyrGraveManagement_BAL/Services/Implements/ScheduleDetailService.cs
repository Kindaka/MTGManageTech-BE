using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class ScheduleDetailService : IScheduleDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public ScheduleDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<string>> CreateScheduleDetail(List<ScheduleDetailDtoRequest> requests, int accountId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var results = new List<string>();

                    foreach (var request in requests)
                    {
                        if (DateOnly.FromDateTime(request.Date) <= DateOnly.FromDateTime(DateTime.Now))
                        {
                            results.Add($"Thời gian để tạo lịch này đã quá hạn (Phải tạo lịch 1 ngày trước khi bắt đầu ngày làm việc).");
                            continue;
                        }
                        var slot = await _unitOfWork.SlotRepository.GetByIDAsync(request.SlotId);
                        if (slot == null)
                        {
                            results.Add($"Slot ID {request.SlotId} không tồn tại.");
                            continue;
                        }

                        var task = await _unitOfWork.TaskRepository.GetByIDAsync(request.TaskId);
                        if (task == null)
                        {
                            results.Add($"Task ID {request.TaskId} không tồn tại.");
                            continue;
                        }
                        if (task.AccountId != accountId)
                        {
                            results.Add($"Task này không phải là của bạn.");
                            continue;
                        }
                        if (task.Status == 4 || task.Status == 5 || task.Status == 2)
                        {
                            results.Add($"Task này đã hoàn thành hoặc đã thất bại hoặc đã hủy.");
                            continue;
                        }

                        var existingScheduleDetails = await _unitOfWork.ScheduleDetailRepository.GetAsync(
                            s => s.SlotId == request.SlotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(request.Date)
                        );
                        var checkTaskInSchedule = false;
                        foreach (var existingTaskInSchedule in existingScheduleDetails)
                        {
                            if (existingTaskInSchedule.TaskId == request.TaskId)
                            {
                                checkTaskInSchedule = true;
                                break;
                            }
                        }
                        if (checkTaskInSchedule) {
                            results.Add($"Task này đã tồn tại trong schedule của bạn rồi.");
                            continue;
                        }

                        if (existingScheduleDetails.Count() < 2)
                        {
                            task.Status = 3;
                            await _unitOfWork.TaskRepository.UpdateAsync(task);
                            var newScheduleDetail = new ScheduleDetail
                            {
                                AccountId = accountId,
                                SlotId = request.SlotId,
                                TaskId = request.TaskId,
                                Date = DateOnly.FromDateTime(request.Date),
                                CreatedAt = DateTime.Now,
                                UpdateAt = DateTime.Now,
                                Status = 1,
                            };
                            await _unitOfWork.ScheduleDetailRepository.AddAsync(newScheduleDetail);

                            var existingAttendances = (await _unitOfWork.AttendanceRepository.GetAsync(
                            s => s.SlotId == request.SlotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(request.Date)
                            )).FirstOrDefault();
                            if (existingAttendances == null)
                            {
                                var attendance = new Attendance
                                {
                                    AccountId = accountId,
                                    SlotId = request.SlotId,
                                    Date = DateOnly.FromDateTime(request.Date),
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    Status = 0 //Attendance ở trạng thái đang chờ
                                };
                                await _unitOfWork.AttendanceRepository.AddAsync(attendance);
                            }
                            await transaction.CommitAsync();
                            results.Add($"Lịch trình đã được tạo thành công.");
                        }
                        else
                        {
                            results.Add($"Lịch trình đã đạt đến tối đa 2 task.");
                            continue;
                        }

                    }
                    
                    return results;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<ScheduleDetailListDtoResponse>> GetScheduleDetailStaff(int accountId, int slotId, DateTime Date)
        {
            try
            {
                var scheduleDetailStaff = await _unitOfWork.ScheduleDetailRepository.GetAsync(sds => sds.SlotId == slotId && sds.AccountId == accountId && sds.Date == DateOnly.FromDateTime(Date), 
                    includeProperties: "Slot,StaffTask.OrderDetail.Service,StaffTask.OrderDetail.MartyrGrave");
                if (scheduleDetailStaff == null)
                {
                    return null;
                }
                var scheduleDetailList = new List<ScheduleDetailListDtoResponse>();
                foreach(var scheduleDetail in scheduleDetailStaff)
                {
                    var scheduleStaff = new ScheduleDetailListDtoResponse
                    {
                        ScheduleDetailId = scheduleDetail.Id,
                        Date = scheduleDetail.Date,
                        StartTime = scheduleDetail.Slot.StartTime,
                        EndTime = scheduleDetail.Slot.EndTime,
                        Description = scheduleDetail.Description,
                        ServiceName = scheduleDetail.StaffTask.OrderDetail.Service.ServiceName,
                        MartyrCode = scheduleDetail.StaffTask.OrderDetail.MartyrGrave.MartyrCode
                    };
                    scheduleDetailList.Add(scheduleStaff);
                }
                return scheduleDetailList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ScheduleDetailListDtoResponse>> GetSchedulesStaff(int accountId, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                var scheduleDetailStaff = await _unitOfWork.ScheduleDetailRepository.GetAsync(sds => sds.AccountId == accountId && sds.Date >= DateOnly.FromDateTime(FromDate) && sds.Date <= DateOnly.FromDateTime(ToDate),
                    includeProperties: "Slot,StaffTask.OrderDetail.Service,StaffTask.OrderDetail.MartyrGrave");
                if (scheduleDetailStaff == null)
                {
                    return null;
                }
                var scheduleDetailList = new List<ScheduleDetailListDtoResponse>();
                foreach (var scheduleDetail in scheduleDetailStaff)
                {
                    var scheduleStaff = new ScheduleDetailListDtoResponse
                    {
                        ScheduleDetailId = scheduleDetail.Id,
                        Date = scheduleDetail.Date,
                        StartTime = scheduleDetail.Slot.StartTime,
                        EndTime = scheduleDetail.Slot.EndTime,
                        Description = scheduleDetail.Description,
                        ServiceName = scheduleDetail.StaffTask.OrderDetail.Service.ServiceName,
                        MartyrCode = scheduleDetail.StaffTask.OrderDetail.MartyrGrave.MartyrCode
                    };
                    scheduleDetailList.Add(scheduleStaff);
                }
                return scheduleDetailList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateScheduleDetail(int slotId, DateTime Date, int accountId, int Id)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (DateOnly.FromDateTime(Date) <= DateOnly.FromDateTime(DateTime.Now))
                    {
                        return "Thời gian để tạo lịch này đã quá hạn (Phải tạo lịch 1 ngày trước khi bắt đầu ngày làm việc).";
                    }
                    var scheduleDetail = (await _unitOfWork.ScheduleDetailRepository.GetAsync(s => s.Id == Id, includeProperties: "StaffTask")).FirstOrDefault();
                    if (scheduleDetail == null) {
                        return "Không tìm thấy task của lịch (Sai Id)";
                    }
                    if (scheduleDetail.StaffTask.Status == 2 || scheduleDetail.StaffTask.Status == 4 || scheduleDetail.StaffTask.Status == 5) {
                        return "Task này đã hoàn thành hoặc đã thất bại hoặc đã hủy.";
                    }
                    var slot = await _unitOfWork.SlotRepository.GetByIDAsync(slotId);
                    if(slot != null)
                    {
                        var checkScheduleDetail = await _unitOfWork.ScheduleDetailRepository.GetAsync(s => s.SlotId == slotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(Date));
                        if (checkScheduleDetail.Count() < 2)
                        {
                            var existingAttendance = (await _unitOfWork.AttendanceRepository.GetAsync(
                            s => s.SlotId == scheduleDetail.SlotId && s.AccountId == accountId && s.Date == scheduleDetail.Date
                            )).FirstOrDefault();
                            var existingScheduleDetail = (await _unitOfWork.ScheduleDetailRepository.GetAsync(
                            s => s.SlotId == scheduleDetail.SlotId && s.AccountId == accountId && s.Date == scheduleDetail.Date && s.Id != Id
                            )).FirstOrDefault();
                            if (existingScheduleDetail == null && existingAttendance != null && existingAttendance.Status == 0)
                            {
                                await _unitOfWork.AttendanceRepository.DeleteAsync(existingAttendance);
                                scheduleDetail.SlotId = slotId;
                                scheduleDetail.Date = DateOnly.FromDateTime(Date);
                                scheduleDetail.UpdateAt = DateTime.Now;
                                await _unitOfWork.ScheduleDetailRepository.UpdateAsync(scheduleDetail);
                                var checkExistingAttendances = (await _unitOfWork.AttendanceRepository.GetAsync(
                           s => s.SlotId == slotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(Date)
                           )).FirstOrDefault();
                                if (checkExistingAttendances == null)
                                {
                                    var attendance = new Attendance
                                    {
                                        AccountId = accountId,
                                        SlotId = slotId,
                                        Date = DateOnly.FromDateTime(Date),
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                        Status = 0 //Attendance ở trạng thái đang chờ
                                    };
                                    await _unitOfWork.AttendanceRepository.AddAsync(attendance);
                                }
                            }
                            else if (existingScheduleDetail != null)
                            {
                                scheduleDetail.SlotId = slotId;
                                scheduleDetail.Date = DateOnly.FromDateTime(Date);
                                scheduleDetail.UpdateAt = DateTime.Now;
                                await _unitOfWork.ScheduleDetailRepository.UpdateAsync(scheduleDetail);
                                var checkExistingAttendances = (await _unitOfWork.AttendanceRepository.GetAsync(
                            s => s.SlotId == slotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(Date)
                            )).FirstOrDefault();
                                if (checkExistingAttendances == null)
                                {
                                    var attendance = new Attendance
                                    {
                                        AccountId = accountId,
                                        SlotId = slotId,
                                        Date = DateOnly.FromDateTime(Date),
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                        Status = 0 //Attendance ở trạng thái đang chờ
                                    };
                                    await _unitOfWork.AttendanceRepository.AddAsync(attendance);
                                }
                            }
                            else
                            {
                                return "Không tìm thấy điểm danh hoặc điểm danh này đã đươc check rồi.";
                            }
                        }
                        else if (checkScheduleDetail.Count() == 2)
                        {
                            return "Đã có tối đa 2 task cho lịch";
                        }
                        await transaction.CommitAsync();
                        return "Lịch trình đã được cập nhật thành công.";
                    }
                    else
                    {
                        return "Không tìm thấy slot";
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
