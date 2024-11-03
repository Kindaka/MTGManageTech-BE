using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

                        var schedule = await _unitOfWork.ScheduleRepository.GetByIDAsync(request.ScheduleId);
                        if (schedule == null)
                        {
                            results.Add($"Schedule ID {request.ScheduleId} không tồn tại.");
                            continue;
                        }

                        var task = await _unitOfWork.TaskRepository.GetByIDAsync(request.TaskId);
                        if (task == null || task.Status != 1 || task.AccountId != accountId)
                        {
                            results.Add($"Task ID {request.TaskId} không tồn tại hoặc task đang làm rồi hoặc task này không phải của bạn.");
                            continue;
                        }
                        if (task.AccountId != accountId)
                        {
                            results.Add($"Task này không phải là của bạn.");
                            continue;
                        }

                        var existingScheduleDetails = await _unitOfWork.ScheduleDetailRepository.GetAsync(
                            s => s.ScheduleId == request.ScheduleId && s.AccountId == accountId
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
                                ScheduleId = request.ScheduleId,
                                TaskId = request.TaskId,
                                CreatedAt = DateTime.Now,
                                Status = 1,
                            };
                            await _unitOfWork.ScheduleDetailRepository.AddAsync(newScheduleDetail);

                            var existingAttendances = (await _unitOfWork.AttendanceRepository.GetAsync(
                            s => s.ScheduleId == request.ScheduleId && s.AccountId == accountId
                            )).FirstOrDefault();
                            if (existingAttendances == null)
                            {
                                var attendance = new Attendance
                                {
                                    AccountId = accountId,
                                    ScheduleId = request.ScheduleId,
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

        public async Task<List<ScheduleDetailListDtoResponse>> GetScheduleDetailStaff(int accountId)
        {
            try
            {
                var scheduleDetailStaff = await _unitOfWork.ScheduleDetailRepository.GetAsync(sds => sds.AccountId == accountId, 
                    includeProperties: "Schedule.Slot,StaffTask.OrderDetail.Service,StaffTask.OrderDetail.MartyrGrave");
                if (scheduleDetailStaff == null)
                {
                    return null;
                }
                var scheduleDetailList = new List<ScheduleDetailListDtoResponse>();
                foreach(var scheduleDetail in scheduleDetailStaff)
                {
                    var scheduleStaff = new ScheduleDetailListDtoResponse
                    {
                        Date = scheduleDetail.Schedule.Date,
                        StartTime = scheduleDetail.Schedule.Slot.StartTime,
                        EndTime = scheduleDetail.Schedule.Slot.EndTime,
                        Description = scheduleDetail.Schedule.Description,
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
    }
}
