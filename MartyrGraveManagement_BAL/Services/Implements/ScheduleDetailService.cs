using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using OfficeOpenXml.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                            s => s.AccountId == accountId && s.Date == DateOnly.FromDateTime(request.Date));
                        if(existingScheduleDetails.Count() <= 10)
                        {
                            task.Status = 3;
                            await _unitOfWork.TaskRepository.UpdateAsync(task);
                            var newScheduleDetail = new ScheduleDetail
                            {
                                AccountId = accountId,
                                TaskId = request.TaskId,
                                Date = DateOnly.FromDateTime(request.Date),
                                CreatedAt = DateTime.Now,
                                UpdateAt = DateTime.Now,
                                ScheduleDetailType = 1, //Loại công việc định kì
                                Status = 1,
                            };
                            await _unitOfWork.ScheduleDetailRepository.AddAsync(newScheduleDetail);
                            results.Add($"Lịch trình đã được tạo thành công.");
                        }
                        else
                        {
                            results.Add($"Một ngày bạn chỉ được thêm tối đa 10 công việc vào lịch làm việc.");
                            continue;
                        }
                        //var existingScheduleDetails = await _unitOfWork.ScheduleDetailRepository.GetAsync(
                        //    s => s.SlotId == request.SlotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(request.Date)
                        //);
                        //var checkTaskInSchedule = false;
                        //foreach (var existingTaskInSchedule in existingScheduleDetails)
                        //{
                        //    if (existingTaskInSchedule.TaskId == request.TaskId)
                        //    {
                        //        checkTaskInSchedule = true;
                        //        break;
                        //    }
                        //}
                        //if (checkTaskInSchedule)
                        //{
                        //    results.Add($"Task này đã tồn tại trong schedule của bạn rồi.");
                        //    continue;
                        //}

                        //if (existingScheduleDetails.Count() < 2)
                        //{
                        //    task.Status = 3;
                        //    await _unitOfWork.TaskRepository.UpdateAsync(task);
                        //    var newScheduleDetail = new ScheduleDetail
                        //    {
                        //        AccountId = accountId,
                        //        SlotId = request.SlotId,
                        //        TaskId = request.TaskId,
                        //        Date = DateOnly.FromDateTime(request.Date),
                        //        CreatedAt = DateTime.Now,
                        //        UpdateAt = DateTime.Now,
                        //        Status = 1,
                        //    };
                        //    await _unitOfWork.ScheduleDetailRepository.AddAsync(newScheduleDetail);

                        //    var existingAttendances = (await _unitOfWork.AttendanceRepository.GetAsync(
                        //    s => s.SlotId == request.SlotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(request.Date)
                        //    )).FirstOrDefault();
                        //    if (existingAttendances == null)
                        //    {
                        //        var attendance = new Attendance
                        //        {
                        //            AccountId = accountId,
                        //            SlotId = request.SlotId,
                        //            Date = DateOnly.FromDateTime(request.Date),
                        //            CreatedAt = DateTime.Now,
                        //            UpdatedAt = DateTime.Now,
                        //            Status = 0 //Attendance ở trạng thái đang chờ
                        //        };
                        //        await _unitOfWork.AttendanceRepository.AddAsync(attendance);
                        //    }
                        //    results.Add($"Lịch trình đã được tạo thành công.");
                        //}
                        //else
                        //{
                        //    results.Add($"Lịch trình đã đạt đến tối đa 2 task.");
                        //    continue;
                        //}

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

        public async Task<List<string>> CreateScheduleDetailForRecurringService(List<ScheduleDetailDtoRequest> requests, int accountId)
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

                        var task = await _unitOfWork.AssignmentTaskRepository.GetByIDAsync(request.TaskId);
                        if (task == null)
                        {
                            results.Add($"Task ID {request.TaskId} không tồn tại.");
                            continue;
                        }
                        if (task.StaffId != accountId)
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
                            s => s.AccountId == accountId && s.Date == DateOnly.FromDateTime(request.Date));
                        if (existingScheduleDetails.Count() <= 10)
                        {
                            task.Status = 3;
                            await _unitOfWork.AssignmentTaskRepository.UpdateAsync(task);
                            var newScheduleDetail = new ScheduleDetail
                            {
                                AccountId = accountId,
                                TaskId = request.TaskId,
                                Date = DateOnly.FromDateTime(request.Date),
                                CreatedAt = DateTime.Now,
                                UpdateAt = DateTime.Now,
                                ScheduleDetailType = 2, //2 là công việc định kì
                                Status = 1,
                            };
                            await _unitOfWork.ScheduleDetailRepository.AddAsync(newScheduleDetail);
                            results.Add($"Lịch trình đã được tạo thành công.");
                        }
                        else
                        {
                            results.Add($"Một ngày bạn chỉ được thêm tối đa 10 công việc vào lịch làm việc.");
                            continue;
                        }
                       

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

        public async Task<string> DeleteScheduleDetail(int accountId, int Id)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var scheduleDetail = (await _unitOfWork.ScheduleDetailRepository.GetAsync(s => s.Id == Id, includeProperties: "Account")).FirstOrDefault();
                    if (scheduleDetail == null)
                    {
                        return "Không tìm thấy task của lịch (Sai Id)";
                    }
                    if (scheduleDetail.ScheduleDetailType == 1)
                    {
                        var taskInSchedule = await _unitOfWork.TaskRepository.GetByIDAsync(scheduleDetail.TaskId);
                        if (taskInSchedule.Status == 2 || taskInSchedule.Status == 4 || taskInSchedule.Status == 5)
                        {
                            return "Task này đã hoàn thành hoặc đã thất bại hoặc đã hủy.";
                        }

                        if (scheduleDetail.Date > DateOnly.FromDateTime(DateTime.Now))
                        {
                            if (scheduleDetail.AccountId == accountId)
                            {
                                await _unitOfWork.ScheduleDetailRepository.DeleteAsync(scheduleDetail);
                                var existingTask = (await _unitOfWork.TaskRepository.GetAsync(t => t.TaskId == scheduleDetail.TaskId)).FirstOrDefault();
                                if (existingTask != null)
                                {
                                    existingTask.Status = 1;
                                    await _unitOfWork.TaskRepository.UpdateAsync(existingTask);
                                }
                            }
                            else
                            {
                                return "Lịch trình này không phải của bạn";
                            }
                        }
                        else
                        {
                            return "Đã quá hạn thời gian để hủy lịch trình (phải cập nhật 1 ngày trước ngày làm việc)";
                        }
                    }
                    else if (scheduleDetail.ScheduleDetailType == 2) {
                        var taskInSchedule = await _unitOfWork.AssignmentTaskRepository.GetByIDAsync(scheduleDetail.TaskId);
                        if (taskInSchedule.Status == 2 || taskInSchedule.Status == 4 || taskInSchedule.Status == 5)
                        {
                            return "Task này đã hoàn thành hoặc đã thất bại hoặc đã hủy.";
                        }

                        if (scheduleDetail.Date > DateOnly.FromDateTime(DateTime.Now))
                        {
                            if (scheduleDetail.AccountId == accountId)
                            {
                                await _unitOfWork.ScheduleDetailRepository.DeleteAsync(scheduleDetail);
                                var existingTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(t => t.AssignmentTaskId == scheduleDetail.TaskId)).FirstOrDefault();
                                if (existingTask != null)
                                {
                                    existingTask.Status = 1;
                                    await _unitOfWork.AssignmentTaskRepository.UpdateAsync(existingTask);
                                }
                            }
                            else
                            {
                                return "Lịch trình này không phải của bạn";
                            }
                        }
                        else
                        {
                            return "Đã quá hạn thời gian để hủy lịch trình (phải cập nhật 1 ngày trước ngày làm việc)";
                        }
                    }
                    await transaction.CommitAsync();
                    return "Lịch trình đã được hủy thành công.";

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<ScheduleDetailForTaskDtoResponse> GetScheduleDetailById(int accountId, int scheduleDetailId)
        {
            try
            {
                var scheduleDetailStaff = (await _unitOfWork.ScheduleDetailRepository.GetAsync(sds => sds.Id == scheduleDetailId,
                    includeProperties: "Account")).FirstOrDefault();


                if (scheduleDetailStaff == null)
                {
                    return null;
                }
                if (scheduleDetailStaff.AccountId != accountId)
                {
                    return null;
                }
                ScheduleDetailForTaskDtoResponse scheduleStaff = new ScheduleDetailForTaskDtoResponse();
                if (scheduleDetailStaff.ScheduleDetailType == 1)
                {
                    var task = (await _unitOfWork.TaskRepository.GetAsync(sds => sds.TaskId == scheduleDetailStaff.TaskId,
                        includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave.Location")).FirstOrDefault();

                    if (task == null)
                    {
                        return null;
                    }


                    scheduleStaff = new ScheduleDetailForTaskDtoResponse
                    {
                        ScheduleDetailId = scheduleDetailStaff.Id,
                        StaffName = scheduleDetailStaff.Account.FullName,
                        Date = scheduleDetailStaff.Date,
                        StartDate = DateOnly.FromDateTime(task.StartDate),
                        EndDate = DateOnly.FromDateTime(task.EndDate),
                        Description = task.Description,
                        ServiceName = task.OrderDetail.Service.ServiceName,
                        ServiceDescription = task.OrderDetail.Service.Description,
                        MartyrCode = task.OrderDetail.MartyrGrave.MartyrCode,
                        TaskId = scheduleDetailStaff.TaskId,
                        ImageWorkSpace = task.ImageWorkSpace,
                        AreaNumber = task.OrderDetail.MartyrGrave.Location.AreaNumber,
                        RowNumber = task.OrderDetail.MartyrGrave.Location.RowNumber,
                        MartyrNumber = task.OrderDetail.MartyrGrave.Location.MartyrNumber,
                        Status = task.Status,
                    };

                    var taskImages = await _unitOfWork.TaskImageRepository.GetAsync(t => t.TaskId == task.TaskId);
                    if (taskImages != null)
                    {
                        foreach (var image in taskImages)
                        {
                            if (image.ImageWorkSpace != null)
                            {
                                var imageTask = new TaskImageDtoResponse
                                {
                                    images = image.ImageWorkSpace,
                                };
                                scheduleStaff.ImageTaskImages.Add(imageTask);
                            }
                        }
                    }
                }
                else if(scheduleDetailStaff.ScheduleDetailType == 2)
                {
                    var task = (await _unitOfWork.AssignmentTaskRepository.GetAsync(sds => sds.AssignmentTaskId == scheduleDetailStaff.TaskId,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave.Location")).FirstOrDefault();

                    if (task == null)
                    {
                        return null;
                    }


                    scheduleStaff = new ScheduleDetailForTaskDtoResponse
                    {
                        ScheduleDetailId = scheduleDetailStaff.Id,
                        StaffName = scheduleDetailStaff.Account.FullName,
                        Date = scheduleDetailStaff.Date,
                        StartDate = DateOnly.FromDateTime(task.CreateAt),
                        EndDate = DateOnly.FromDateTime(task.EndDate),
                        Description = task.Description,
                        ServiceName = task.Service_Schedule.Service.ServiceName,
                        ServiceDescription = task.Service_Schedule.Service.Description,
                        MartyrCode = task.Service_Schedule.MartyrGrave.MartyrCode,
                        AssignmentTaskId = scheduleDetailStaff.TaskId,
                        ImageWorkSpace = task.ImageWorkSpace,
                        AreaNumber = task.Service_Schedule.MartyrGrave.Location.AreaNumber,
                        RowNumber = task.Service_Schedule.MartyrGrave.Location.RowNumber,
                        MartyrNumber = task.Service_Schedule.MartyrGrave.Location.MartyrNumber,
                        Status = task.Status,
                    };

                    var taskImages = await _unitOfWork.AssignmentTaskImageRepository.GetAsync(t => t.AssignmentTaskId == task.AssignmentTaskId);
                    if (taskImages != null)
                    {
                        foreach (var image in taskImages)
                        {
                            if (image.ImagePath != null)
                            {
                                var imageTask = new TaskImageDtoResponse
                                {
                                    images = image.ImagePath,
                                };
                                scheduleStaff.ImageTaskImages.Add(imageTask);
                            }
                        }
                    }
                }

                return scheduleStaff;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ScheduleDetailListDtoResponse>> GetScheduleDetailStaff(int accountId, DateTime Date)
        {
            try
            {
                var scheduleDetailStaff = await _unitOfWork.ScheduleDetailRepository.GetAsync(sds => sds.AccountId == accountId && sds.Date == DateOnly.FromDateTime(Date));
                if (scheduleDetailStaff == null)
                {
                    return null;
                }
                var scheduleDetailList = new List<ScheduleDetailListDtoResponse>();
                foreach (var scheduleDetail in scheduleDetailStaff)
                {
                    if (scheduleDetail.ScheduleDetailType == 1)
                    {
                        var task = await _unitOfWork.TaskRepository.GetByIDAsync(scheduleDetail.TaskId);
                        var scheduleStaff = new ScheduleDetailListDtoResponse
                        {
                            ScheduleDetailId = scheduleDetail.Id,
                            Date = scheduleDetail.Date,
                            Description = scheduleDetail.Description,
                            ServiceName = task.OrderDetail.Service.ServiceName,
                            MartyrCode = task.OrderDetail.MartyrGrave.MartyrCode
                        };
                        scheduleDetailList.Add(scheduleStaff);
                    }
                    else if (scheduleDetail.ScheduleDetailType == 2) {
                        var task = await _unitOfWork.AssignmentTaskRepository.GetByIDAsync(scheduleDetail.TaskId);
                        var scheduleStaff = new ScheduleDetailListDtoResponse
                        {
                            ScheduleDetailId = scheduleDetail.Id,
                            Date = scheduleDetail.Date,
                            Description = scheduleDetail.Description,
                            ServiceName = task.Service_Schedule.Service.ServiceName,
                            MartyrCode = task.Service_Schedule.MartyrGrave.MartyrCode
                        };
                        scheduleDetailList.Add(scheduleStaff);
                    }
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
                var scheduleDetailStaff = await _unitOfWork.ScheduleDetailRepository.GetAsync(sds => sds.AccountId == accountId && sds.Date >= DateOnly.FromDateTime(FromDate) && sds.Date <= DateOnly.FromDateTime(ToDate));
                if (scheduleDetailStaff == null)
                {
                    return null;
                }
                var scheduleDetailList = new List<ScheduleDetailListDtoResponse>();
                foreach (var scheduleDetail in scheduleDetailStaff)
                {
                    if (scheduleDetail.ScheduleDetailType == 1)
                    {
                        var task = (await _unitOfWork.TaskRepository.GetAsync(t => t.TaskId == scheduleDetail.TaskId, includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave")).FirstOrDefault();
                        var scheduleStaff = new ScheduleDetailListDtoResponse
                        {
                            ScheduleDetailId = scheduleDetail.Id,
                            Date = scheduleDetail.Date,
                            Description = scheduleDetail.Description,
                            ServiceName = task.OrderDetail.Service.ServiceName,
                            MartyrCode = task.OrderDetail.MartyrGrave.MartyrCode,
                            Status = task.Status
                        };
                        scheduleDetailList.Add(scheduleStaff);
                    }
                    else if (scheduleDetail.ScheduleDetailType == 2) {
                        var task = (await _unitOfWork.AssignmentTaskRepository.GetAsync(t => t.AssignmentTaskId == scheduleDetail.TaskId, includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave")).FirstOrDefault();
                        var scheduleStaff = new ScheduleDetailListDtoResponse
                        {
                            ScheduleDetailId = scheduleDetail.Id,
                            Date = scheduleDetail.Date,
                            Description = scheduleDetail.Description,
                            ServiceName = task.Service_Schedule.Service.ServiceName,
                            MartyrCode = task.Service_Schedule.MartyrGrave.MartyrCode,
                            Status = task.Status
                        };
                        scheduleDetailList.Add(scheduleStaff);
                    }
                    
                }
                return scheduleDetailList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public async Task<string> UpdateScheduleDetail(int slotId, DateTime Date, int accountId, int Id)
        //{
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            if (DateOnly.FromDateTime(Date) <= DateOnly.FromDateTime(DateTime.Now))
        //            {
        //                return "Thời gian để tạo lịch này đã quá hạn (Phải tạo lịch 1 ngày trước khi bắt đầu ngày làm việc).";
        //            }
        //            var scheduleDetail = (await _unitOfWork.ScheduleDetailRepository.GetAsync(s => s.Id == Id, includeProperties: "StaffTask")).FirstOrDefault();
        //            if (scheduleDetail == null)
        //            {
        //                return "Không tìm thấy task của lịch (Sai Id)";
        //            }
        //            if (scheduleDetail.StaffTask.Status == 2 || scheduleDetail.StaffTask.Status == 4 || scheduleDetail.StaffTask.Status == 5)
        //            {
        //                return "Task này đã hoàn thành hoặc đã thất bại hoặc đã hủy.";
        //            }
        //            var slot = await _unitOfWork.SlotRepository.GetByIDAsync(slotId);
        //            if (slot != null)
        //            {
        //                var checkScheduleDetail = await _unitOfWork.ScheduleDetailRepository.GetAsync(s => s.SlotId == slotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(Date));
        //                if (checkScheduleDetail.Count() < 2)
        //                {
        //                    var existingAttendance = (await _unitOfWork.AttendanceRepository.GetAsync(
        //                    s => s.SlotId == scheduleDetail.SlotId && s.AccountId == accountId && s.Date == scheduleDetail.Date
        //                    )).FirstOrDefault();
        //                    var existingScheduleDetail = (await _unitOfWork.ScheduleDetailRepository.GetAsync(
        //                    s => s.SlotId == scheduleDetail.SlotId && s.AccountId == accountId && s.Date == scheduleDetail.Date && s.Id != Id
        //                    )).FirstOrDefault();
        //                    if (existingScheduleDetail == null && existingAttendance != null && existingAttendance.Status == 0)
        //                    {
        //                        await _unitOfWork.AttendanceRepository.DeleteAsync(existingAttendance);
        //                        scheduleDetail.SlotId = slotId;
        //                        scheduleDetail.Date = DateOnly.FromDateTime(Date);
        //                        scheduleDetail.UpdateAt = DateTime.Now;
        //                        await _unitOfWork.ScheduleDetailRepository.UpdateAsync(scheduleDetail);
        //                        var checkExistingAttendances = (await _unitOfWork.AttendanceRepository.GetAsync(
        //                   s => s.SlotId == slotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(Date)
        //                   )).FirstOrDefault();
        //                        if (checkExistingAttendances == null)
        //                        {
        //                            var attendance = new Attendance
        //                            {
        //                                AccountId = accountId,
        //                                SlotId = slotId,
        //                                Date = DateOnly.FromDateTime(Date),
        //                                CreatedAt = DateTime.Now,
        //                                UpdatedAt = DateTime.Now,
        //                                Status = 0 //Attendance ở trạng thái đang chờ
        //                            };
        //                            await _unitOfWork.AttendanceRepository.AddAsync(attendance);
        //                        }
        //                    }
        //                    else if (existingScheduleDetail != null)
        //                    {
        //                        scheduleDetail.SlotId = slotId;
        //                        scheduleDetail.Date = DateOnly.FromDateTime(Date);
        //                        scheduleDetail.UpdateAt = DateTime.Now;
        //                        await _unitOfWork.ScheduleDetailRepository.UpdateAsync(scheduleDetail);
        //                        var checkExistingAttendances = (await _unitOfWork.AttendanceRepository.GetAsync(
        //                    s => s.SlotId == slotId && s.AccountId == accountId && s.Date == DateOnly.FromDateTime(Date)
        //                    )).FirstOrDefault();
        //                        if (checkExistingAttendances == null)
        //                        {
        //                            var attendance = new Attendance
        //                            {
        //                                AccountId = accountId,
        //                                SlotId = slotId,
        //                                Date = DateOnly.FromDateTime(Date),
        //                                CreatedAt = DateTime.Now,
        //                                UpdatedAt = DateTime.Now,
        //                                Status = 0 //Attendance ở trạng thái đang chờ
        //                            };
        //                            await _unitOfWork.AttendanceRepository.AddAsync(attendance);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return "Không tìm thấy điểm danh hoặc điểm danh này đã đươc check rồi.";
        //                    }
        //                }
        //                else if (checkScheduleDetail.Count() == 2)
        //                {
        //                    return "Đã có tối đa 2 task cho lịch";
        //                }
        //                await transaction.CommitAsync();
        //                return "Lịch trình đã được cập nhật thành công.";
        //            }
        //            else
        //            {
        //                return "Không tìm thấy slot";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            throw new Exception(ex.Message);
        //        }
        //    }
        //}
    }
}
