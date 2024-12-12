using AutoMapper;
using DocumentFormat.OpenXml.ExtendedProperties;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDtoRequest;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;
using MartyrGraveManagement_BAL.ModelViews.StaffDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AssignmentTaskService : IAssignmentTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AssignmentTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateTasksAsync(int serviceScheduleId)
        {
            try
            {
                //var taskResponses = new List<TaskDtoResponse>();


                    // Kiểm tra xem OrderId có tồn tại không
                    var serviceSchedule = (await _unitOfWork.ServiceScheduleRepository.GetAsync(s => s.ServiceScheduleId == serviceScheduleId, includeProperties:"Service")).FirstOrDefault();
                    if (serviceSchedule == null)
                    {
                        throw new KeyNotFoundException("Id không tồn tại.");
                    }

                    // Kiểm tra trạng thái của Order, nếu không phải là 1 thì không cho tạo Task
                    if (serviceSchedule.Status != true)
                    {
                        throw new InvalidOperationException("Đơn hàng không ở trạng thái hợp lệ để tạo Task.");
                    }


                    // Kiểm tra xem khu vực của MartyrGrave trong OrderDetail
                    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(serviceSchedule.MartyrId);
                    if (martyrGrave == null)
                    {
                        throw new InvalidOperationException("MartyrGrave không tồn tại.");
                    }

                    // Lấy danh sách nhân viên thuộc cùng khu vực
                    var staffAccounts = await _unitOfWork.AccountRepository
                        .FindAsync(a => a.RoleId == 3 && a.AreaId == martyrGrave.AreaId);

                    if (!staffAccounts.Any())
                    {
                        throw new InvalidOperationException("Không có nhân viên nào thuộc khu vực phù hợp để nhận công việc.");
                    }

                    // Sắp xếp danh sách nhân viên dựa trên số lượng công việc
                    var staffWorkloads = new Dictionary<int, int>();
                    foreach (var staff in staffAccounts)
                    {
                        var taskCount = await _unitOfWork.AssignmentTaskRepository
                            .CountAsync(t => t.StaffId == staff.AccountId); // Lấy công việc đang được chỉ định
                        staffWorkloads[staff.AccountId] = taskCount;
                    }

                    var sortedStaffAccounts = staffAccounts
                        .OrderBy(staff => staffWorkloads[staff.AccountId])
                        .ToList();

                    // Lấy nhân viên có ít công việc nhất
                    var selectedStaff = sortedStaffAccounts.First();

                    DateTime? serviceDate = null;

                    if (serviceSchedule.Service.RecurringType == 1)
                    {
                        serviceDate = GetNextWeeklyServiceDate(serviceSchedule.DayOfWeek);
                    }
                    else if (serviceSchedule.Service.RecurringType == 2)
                    {
                        serviceDate = GetNextMonthlyServiceDate(serviceSchedule.DayOfMonth);
                    }

                    // Tạo task mới và gắn Note của Order vào Description
                    var taskEntity = new AssignmentTask
                    {
                        StaffId = selectedStaff.AccountId,
                        ServiceScheduleId = serviceScheduleId,
                        CreateAt = DateTime.Now,
                        EndDate = (DateTime)serviceDate,
                        Description = serviceSchedule.Note,  // Gắn Note của Order vào Description của Task
                        Status = 1  // Trạng thái ban đầu là 'assigned'
                    };

                    // Thêm Task vào cơ sở dữ liệu
                    await _unitOfWork.AssignmentTaskRepository.AddAsync(taskEntity);

                    
                

                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DateTime GetNextWeeklyServiceDate(int dayOfService)
        {
            var today = DateTime.Now;
            int currentDayOfWeek = (int)today.DayOfWeek; // 7 = Chủ Nhật, ..., 6 = Thứ Bảy

            // Điều chỉnh để phù hợp với DayOfService (1 = Thứ 2, ..., 7 = Chủ nhật)
            currentDayOfWeek = currentDayOfWeek == 0 ? 7 : currentDayOfWeek; // Chủ Nhật => 7

            int daysUntilNextService = (dayOfService - currentDayOfWeek + 7) % 7;
            return today.AddDays(daysUntilNextService == 0 ? 7 : daysUntilNextService); // Nếu trùng ngày, lấy tuần sau
        }

        public DateTime? GetNextMonthlyServiceDate(int dayOfService)
        {
            var today = DateTime.Now;

            // Lấy ngày trong tháng hiện tại
            int currentDay = today.Day;

            // Nếu ngày hiện tại < DayOfService, dịch vụ sẽ diễn ra trong tháng này
            if (currentDay < dayOfService)
            {
                return new DateTime(today.Year, today.Month, dayOfService);
            }

            // Nếu không, chuyển sang tháng tiếp theo
            int nextMonth = today.Month + 1;
            int year = today.Year;

            if (nextMonth > 12)
            {
                nextMonth = 1;
                year++;
            }

            // Kiểm tra nếu tháng tiếp theo có đủ số ngày
            int daysInNextMonth = DateTime.DaysInMonth(year, nextMonth);
            if (dayOfService > daysInNextMonth)
            {
                return null; // Không thể lập lịch do ngày vượt quá số ngày trong tháng
            }

            return new DateTime(year, nextMonth, dayOfService);
        }

        public async Task<AssignmentTaskResponse> UpdateTaskStatusAsync(int taskId, AssignmentTaskStatusUpdateDTO updateDto)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra TaskId có tồn tại không
                    var task = (await _unitOfWork.AssignmentTaskRepository.GetAsync(t => t.AssignmentTaskId == taskId, includeProperties:"Account,Service_Schedule.Service")).FirstOrDefault();
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }

                    // 2. Cập nhật trạng thái của Task
                    if (task.Status == 1)
                    {
                        if (updateDto.Status == 2) // Từ chối task
                        {
                            // Kiểm tra lý do từ chối
                            if (string.IsNullOrWhiteSpace(updateDto.Reason))
                            {
                                throw new InvalidOperationException("Reason is required when rejecting a task.");
                            }

                            task.Status = 2;
                            task.Reason = updateDto.Reason;
                        }
                        else
                        {
                            throw new InvalidOperationException("You can only update status to 2 (reject) from status 1.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid status transition.");
                    }

                    // 3. Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.AssignmentTaskRepository.UpdateAsync(task);
                    if (task.Account.AreaId != null)
                    {
                        var manager = (await _unitOfWork.AccountRepository.GetAsync(m => m.RoleId == 2 && m.AreaId == task.Account.AreaId)).FirstOrDefault();
                        if (manager != null)
                        {
                            await CreateNotification(
                    "Một công việc đã bị từ chối bởi nhân viên",
                    $"Công việc định kì {task.Service_Schedule?.Service?.ServiceName} đã bị từ chối bởi {task.Account.FullName}. Hãy kiểm tra lại công việc đó",
                    manager.AccountId, "/task-manager"
                );
                        }
                    }

                    // 4. Commit transaction
                    await transaction.CommitAsync();

                    // Lấy task với đầy đủ thông tin liên quan để map
                    var updatedTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.AssignmentTaskId == taskId,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages"
                    )).FirstOrDefault();

                    return _mapper.Map<AssignmentTaskResponse>(updatedTask);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to update task: {ex.Message}");
                }
            }
        }

        public async Task<AssignmentTaskResponse> UpdateTaskImagesAsync(int taskId, AssignmentTaskImageUpdateDTO imageUpdateDto)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var task = (await _unitOfWork.AssignmentTaskRepository.GetAsync(t => t.AssignmentTaskId == taskId, includeProperties: "Service_Schedule.Account,Service_Schedule.Service,Account")).FirstOrDefault();
                    if (task == null)
                    {
                        throw new KeyNotFoundException("Task not found.");
                    }

                    // Kiểm tra trạng thái task
                    if (task.Status != 3) // Nếu task chưa ở trạng thái "in progress"
                    {
                        throw new InvalidOperationException("Task must be in progress to update images.");
                    }

                    // Cập nhật ImageWorkSpace và trạng thái
                    task.ImageWorkSpace = imageUpdateDto.ImageWorkSpace;
                    task.Status = 4; // Cập nhật trạng thái thành "completed"
                    task.CreateAt = DateTime.Now; // Thêm thời gian hoàn thành

                    // Xóa các hình ảnh cũ
                    var oldImages = await _unitOfWork.AssignmentTaskImageRepository
                        .FindAsync(i => i.AssignmentTaskId == taskId);
                    foreach (var oldImage in oldImages)
                    {
                        await _unitOfWork.AssignmentTaskImageRepository.DeleteAsync(oldImage);
                    }

                    // Thêm các hình ảnh mới
                    foreach (var url in imageUpdateDto.UrlImages)
                    {
                        var newImage = new AssignmentTaskImage
                        {
                            AssignmentTaskId = taskId,
                            ImagePath = url,
                            CreateAt = DateTime.Now
                        };
                        await _unitOfWork.AssignmentTaskImageRepository.AddAsync(newImage);
                    }

                    // Cập nhật task
                    await _unitOfWork.AssignmentTaskRepository.UpdateAsync(task);
                    await CreateNotification(
                    "Công việc định kì đã được hoàn thành",
                    $"Công việc định kì {task.Service_Schedule?.Service?.ServiceName} đã được hoàn thành bởi {task.Account?.FullName}. Khách hàng có thể kiểm tra lại và cho phản hồi",
                    task.Service_Schedule.AccountId, $"/schedule-service-detail/{task.ServiceScheduleId}"
                );
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    // Lấy task với đầy đủ thông tin liên quan để map
                    var updatedTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.AssignmentTaskId == taskId,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages"
                    )).FirstOrDefault();

                    return _mapper.Map<AssignmentTaskResponse>(updatedTask);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to update task images: {ex.Message}");
                }
            }
        }

        public async Task<AssignmentTaskResponse> ReassignTaskAsync(int taskId, int newStaffId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra xem TaskId có tồn tại không
                    var task = await _unitOfWork.AssignmentTaskRepository.GetByIDAsync(taskId);
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }

                    // 2. Kiểm tra trạng thái hiện tại của Task
                    if (task.Status != 2)
                    {
                        throw new InvalidOperationException("Task can only be reassigned if it is in status 2 (rejected).");
                    }

                    // 3. Kiểm tra xem StaffId mới có tồn tại không
                    var newStaff = await _unitOfWork.AccountRepository.GetByIDAsync(newStaffId);
                    if (newStaff == null)
                    {
                        throw new KeyNotFoundException("New StaffId does not exist.");
                    }

                    // 4. Kiểm tra Role của staff mới phải là Role 3 (Staff)
                    if (newStaff.RoleId != 3)
                    {
                        throw new UnauthorizedAccessException("The new account is not authorized to perform this task (not a staff account).");
                    }

                    // 5. Cập nhật StaffId mới và Status của task
                    task.StaffId = newStaffId;
                    task.Status = 1; // Trạng thái về 1 (đã bàn giao)

                    // 6. Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.AssignmentTaskRepository.UpdateAsync(task);
                    await CreateNotification(
                    "Một công việc mới đã được giao lại bởi quản lý",
                    $"Công việc định kì {task.Service_Schedule?.Service?.ServiceName} đã được giao lại cho nhân viên {task.Account.FullName}. Hãy kiểm tra lại công việc đó",
                    task.StaffId, "/recurring-tasks"
                    );
                    await _unitOfWork.SaveAsync();

                    // 7. Commit transaction
                    await transaction.CommitAsync();

                    // Lấy task với đầy đủ thông tin liên quan để map
                    var updatedTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.AssignmentTaskId == taskId,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages"
                    )).FirstOrDefault();

                    return _mapper.Map<AssignmentTaskResponse>(updatedTask);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to reassign task: {ex.Message}");
                }
            }
        }

        public async Task<(IEnumerable<AssignmentTaskResponse> taskList, int totalPage)> GetAssignmentTasksByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                var taskResponses = new List<AssignmentTaskResponse>();
                // Kiểm tra xem AccountId có tồn tại không
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }

                int totalPage = 0;
                int totalTask = 0;
                IEnumerable<AssignmentTask> tasks;

                if (Date == DateTime.MinValue)
                {
                    totalTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(s => s.StaffId == accountId)).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    tasks = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.StaffId == accountId,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages",
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        s => s.StaffId == accountId && s.EndDate.Date == Date.Date)).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    tasks = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.StaffId == accountId && t.EndDate.Date == Date.Date,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages",
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }

                if (!tasks.Any())
                {
                    return (new List<AssignmentTaskResponse>(), 0);
                }

                // Lấy thông tin vị trí cho tất cả các task
                foreach (var task in tasks)
                {
                    var taskAssignment = _mapper.Map<AssignmentTaskResponse>(task);
                    // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                    var martyrGrave = task.Service_Schedule?.MartyrGrave;
                    if (martyrGrave != null)
                    {
                        var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                        taskAssignment.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                    }
                    var service = task.Service_Schedule.Service;
                    if (service != null)
                    {
                        var serviceImage = await _unitOfWork.ServiceRepository.GetByIDAsync(service.ServiceId);
                        taskAssignment.ServiceImage = serviceImage.ImagePath;
                    }
                    taskResponses.Add(taskAssignment);
                }

                
                return (taskResponses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IEnumerable<AssignmentTaskResponse> taskList, int totalPage)> GetAssignmentTasksForManager(int managerId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                // Kiểm tra xem Manager có tồn tại không
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if (manager == null || manager.RoleId != 2)
                {
                    throw new KeyNotFoundException("Manager not found or invalid role.");
                }

                int totalPage = 0;
                int totalTask = 0;
                IEnumerable<AssignmentTask> tasks = new List<AssignmentTask>();

                if (Date == DateTime.MinValue)
                {
                    totalTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        s => s.Service_Schedule.MartyrGrave.AreaId == manager.AreaId, 
                        includeProperties: "Service_Schedule.MartyrGrave")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    tasks = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        s => s.Service_Schedule.MartyrGrave.AreaId == manager.AreaId,
                        includeProperties: "Service_Schedule.Service.ServiceCategory,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages",
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        s => s.Service_Schedule.MartyrGrave.AreaId == manager.AreaId && 
                        s.EndDate.Date == Date.Date,
                        includeProperties: "Service_Schedule.MartyrGrave")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    tasks = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.Service_Schedule.MartyrGrave.AreaId == manager.AreaId && 
                        t.EndDate.Date == Date.Date,
                        includeProperties: "Service_Schedule.Service.ServiceCategory,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages",
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }

                if (!tasks.Any())
                {
                    return (new List<AssignmentTaskResponse>(), 0);
                }
                var responses = new List<AssignmentTaskResponse>(); 
                // Lấy thông tin vị trí cho tất cả các task
                foreach (var task in tasks)
                {
                    var taskReponse = _mapper.Map<AssignmentTaskResponse>(task);
                    taskReponse.ServiceName = task.Service_Schedule?.Service?.ServiceName;
                    taskReponse.ServiceImage = task.Service_Schedule?.Service?.ImagePath;
                    taskReponse.CategoryName = task.Service_Schedule?.Service?.ServiceCategory?.CategoryName;
                    var martyr = task.Service_Schedule?.MartyrGrave;
                    if (martyr != null)
                    {
                        var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyr.LocationId);
                        if (location != null)
                        {
                            taskReponse.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                        }
                    }
                    if (taskReponse.Status == 2)
                    {
                        if (manager.AreaId == task.Service_Schedule?.MartyrGrave?.AreaId)
                        {
                            var accountStaffs = await _unitOfWork.AccountRepository.GetAsync(s => s.AreaId == task.Service_Schedule.MartyrGrave.AreaId && s.RoleId == 3);
                            if (accountStaffs != null)
                            {
                                foreach (var accountStaff in accountStaffs)
                                {
                                    if (accountStaff.Status == true)
                                    {
                                        var staffDto = new StaffDtoResponse
                                        {
                                            AccountId = accountStaff.AccountId,
                                            StaffFullName = accountStaff.FullName
                                        };
                                        taskReponse.Staffs?.Add(staffDto);
                                    }
                                }
                            }
                        }
                    }
                    responses.Add(taskReponse);
                }
                

                //var responses = tasks.Select(t => _mapper.Map<AssignmentTaskResponse>(t)).ToList();
                return (responses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AssignmentTaskResponse> GetAssignmentTaskByIdAsync(int taskId)
        {
            try
            {
                // Lấy task với các thông tin liên quan
                var task = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                    t => t.AssignmentTaskId == taskId,
                    includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages"
                )).FirstOrDefault();

                if (task == null)
                {
                    throw new KeyNotFoundException("Task not found.");
                }

                // Lấy thông tin vị trí mộ
                var martyr = task.Service_Schedule?.MartyrGrave;
                if (martyr != null)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyr.LocationId);
                    if (location != null)
                    {
                        task.Service_Schedule.MartyrGrave.Location = location;
                    }
                }

                return _mapper.Map<AssignmentTaskResponse>(task);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(IEnumerable<AssignmentTaskResponse> taskList, int totalPage)> GetAssignmentTasksNotSchedulingByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                var taskResponses = new List<AssignmentTaskResponse>();
                // Kiểm tra xem AccountId có tồn tại không
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }

                int totalPage = 0;
                int totalTask = 0;
                IEnumerable<AssignmentTask> tasks;

                if (Date == DateTime.MinValue)
                {
                    totalTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(s => s.StaffId == accountId && s.Status == 1)).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    tasks = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.StaffId == accountId && t.Status == 1,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages",
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }
                else
                {
                    totalTask = (await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        s => s.StaffId == accountId && s.EndDate.Date == Date.Date && s.Status == 1)).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                    tasks = await _unitOfWork.AssignmentTaskRepository.GetAsync(
                        t => t.StaffId == accountId && t.EndDate.Date == Date.Date && t.Status == 1,
                        includeProperties: "Service_Schedule.Service,Service_Schedule.MartyrGrave,Service_Schedule.Account,Account,AssignmentTaskImages",
                        pageIndex: pageIndex,
                        pageSize: pageSize
                    );
                }

                if (!tasks.Any())
                {
                    return (new List<AssignmentTaskResponse>(), 0);
                }

                // Lấy thông tin vị trí cho tất cả các task
                foreach (var task in tasks)
                {
                    var taskAssignment = _mapper.Map<AssignmentTaskResponse>(task);
                    // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                    var martyrGrave = task.Service_Schedule?.MartyrGrave;
                    if (martyrGrave != null)
                    {
                        var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                        taskAssignment.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                    }
                    var service = task.Service_Schedule.Service;
                    if (service != null)
                    {
                        var serviceImage = await _unitOfWork.ServiceRepository.GetByIDAsync(service.ServiceId);
                        taskAssignment.ServiceImage = serviceImage.ImagePath;
                    }
                    taskResponses.Add(taskAssignment);
                }


                return (taskResponses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task CreateNotification(string title, string description, int accountId, string linkTo)
        {
            // Tạo thông báo
            var notification = new Notification
            {
                Title = title,
                Description = description,
                CreatedDate = DateTime.Now,
                LinkTo = linkTo,
                Status = true
            };
            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveAsync();

            // Liên kết thông báo với tài khoản
            var notificationAccount = new NotificationAccount
            {
                AccountId = accountId,
                NotificationId = notification.NotificationId,
                Status = true
            };
            await _unitOfWork.NotificationAccountsRepository.AddAsync(notificationAccount);
        }
    }
}
