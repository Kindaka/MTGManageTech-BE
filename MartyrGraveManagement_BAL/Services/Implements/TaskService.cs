using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.Identity.Client;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskDtoResponse>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.TaskRepository.GetAsync(includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave");

            if (!tasks.Any())
            {
                throw new KeyNotFoundException("No tasks found.");
            }

            // Lấy thông tin FullName từ Account và ánh xạ sang TaskDtoResponse
            var taskResponses = new List<TaskDtoResponse>();
            foreach (var task in tasks)
            {
                // Lấy thông tin Account để ánh xạ Fullname
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(task.AccountId);

                // Ánh xạ task sang TaskDtoResponse
                var taskDto = _mapper.Map<TaskDtoResponse>(task);
                taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

                // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                taskDto.ServiceName = task.OrderDetail?.Service?.ServiceName;
                taskDto.ServiceDescription = task.OrderDetail?.Service?.Description;

                // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                var martyrGrave = task.OrderDetail?.MartyrGrave;
                if (martyrGrave != null)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                    taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                }

                taskResponses.Add(taskDto);
            }

            return taskResponses;
        }






        public async Task<(IEnumerable<TaskDtoResponse> taskList, int totalPage)> GetTasksByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date)
        {
            // Kiểm tra xem AccountId có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            int totalPage = 0;
            int totalTask = 0;
            IEnumerable<StaffTask> tasks = new List<StaffTask>();

            if (Date == DateTime.MinValue)
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.AccountId == accountId)).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3
                tasks = await _unitOfWork.TaskRepository.GetAsync(
                    t => t.AccountId == accountId,
                    includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
            }
            else
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.AccountId == accountId && s.StartDate.Date == Date)).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3 theo ngày
                tasks = await _unitOfWork.TaskRepository.GetAsync(
                    t => t.AccountId == accountId && t.StartDate.Date == Date,
                    includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
            }

            // Nếu không có Task nào
            if (!tasks.Any())
            {
                throw new InvalidOperationException("This account does not have any tasks.");
            }

            // Ánh xạ FullName từ Account và các thông tin khác
            var taskResponses = new List<TaskDtoResponse>();
            foreach (var task in tasks)
            {
                var taskDto = _mapper.Map<TaskDtoResponse>(task);
                taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

                // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                taskDto.ServiceName = task.OrderDetail?.Service?.ServiceName;
                taskDto.ServiceDescription = task.OrderDetail?.Service?.Description;

                // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                var martyrGrave = task.OrderDetail?.MartyrGrave;
                if (martyrGrave != null)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                    taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                }

                taskResponses.Add(taskDto);
            }

            return (taskResponses, totalPage);
        }

        public async Task<(IEnumerable<TaskDtoResponse> taskList, int totalPage)> GetTasksNotSchedulingByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date)
        {
            // Kiểm tra xem AccountId có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            int totalPage = 0;
            int totalTask = 0;
            IEnumerable<StaffTask> tasks = new List<StaffTask>();

            if (Date == DateTime.MinValue)
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.AccountId == accountId && (s.Status == 1))).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3
                tasks = await _unitOfWork.TaskRepository.GetAsync(
                    t => t.AccountId == accountId && (t.Status == 1),
                    includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
            }
            else
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.AccountId == accountId && s.StartDate.Date == Date && (s.Status == 1))).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3 theo ngày
                tasks = await _unitOfWork.TaskRepository.GetAsync(
                    t => t.AccountId == accountId && t.StartDate.Date == Date && (t.Status == 1),
                    includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
            }

            // Nếu không có Task nào
            if (!tasks.Any())
            {
                throw new InvalidOperationException("This account does not have any tasks.");
            }

            // Ánh xạ FullName từ Account và các thông tin khác
            var taskResponses = new List<TaskDtoResponse>();
            foreach (var task in tasks)
            {
                var taskDto = _mapper.Map<TaskDtoResponse>(task);
                taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

                // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                taskDto.ServiceName = task.OrderDetail?.Service?.ServiceName;
                taskDto.ServiceDescription = task.OrderDetail?.Service?.Description;

                // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                var martyrGrave = task.OrderDetail?.MartyrGrave;
                if (martyrGrave != null)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                    taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                }

                taskResponses.Add(taskDto);
            }

            return (taskResponses, totalPage);
        }


        public async Task<(IEnumerable<TaskDtoResponse> taskList, int totalPage)> GetTasksForManager(int managerId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                // Kiểm tra xem AccountId có tồn tại không
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }
                var taskResponses = new List<TaskDtoResponse>();
                int totalPage = 0;
                int totalTask = 0;
                IEnumerable<StaffTask> tasks = new List<StaffTask>();
                if (Date == DateTime.MinValue)
                {
                    totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.OrderDetail.MartyrGrave.AreaId == account.AreaId, includeProperties: "OrderDetail.MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    // Lấy tất cả các đơn hàng dựa trên AccountId và bao gồm các chi tiết đơn hàng
                    tasks = await _unitOfWork.TaskRepository.GetAsync(s => s.OrderDetail.MartyrGrave.AreaId == account.AreaId, includeProperties: "OrderDetail.Service.ServiceCategory,OrderDetail.MartyrGrave,Account",
                    pageIndex: pageIndex, pageSize: pageSize);
                }
                else
                {
                    totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.OrderDetail.MartyrGrave.AreaId == account.AreaId && s.StartDate.Date == Date.Date, includeProperties: "OrderDetail.MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    // Lấy tất cả các đơn hàng dựa trên AccountId và bao gồm các chi tiết đơn hàng
                    tasks = await _unitOfWork.TaskRepository.GetAsync(t => t.OrderDetail.MartyrGrave.AreaId == account.AreaId && t.StartDate.Date == Date.Date, includeProperties: "OrderDetail.Service.ServiceCategory,OrderDetail.MartyrGrave,Account",
                    pageIndex: pageIndex, pageSize: pageSize);
                }


                // Lấy danh sách các Task thuộc về account, bao gồm các bảng liên quan


                if (!tasks.Any())
                {
                    return (taskResponses, 0);
                }



                foreach (var task in tasks)
                {
                    var taskDto = _mapper.Map<TaskDtoResponse>(task);
                    taskDto.Fullname = task.Account?.FullName;  // Ánh xạ FullName từ Account

                    // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                    taskDto.ServiceName = task.OrderDetail?.Service?.ServiceName;
                    taskDto.ServiceImage = task.OrderDetail?.Service?.ImagePath;
                    taskDto.ServiceDescription = task.OrderDetail?.Service?.Description;
                    taskDto.CategoryName = task.OrderDetail?.Service?.ServiceCategory?.CategoryName;

                    // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                    var martyrGrave = task.OrderDetail?.MartyrGrave;
                    if (martyrGrave != null)
                    {
                        var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                        taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                    }

                    taskResponses.Add(taskDto);
                }

                return (taskResponses, totalPage);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }






        public async Task<TaskDtoResponse> GetTaskByIdAsync(int taskId)
        {
            // Lấy thông tin Task theo taskId, bao gồm các bảng liên quan
            var task = await _unitOfWork.TaskRepository.GetAsync(t => t.TaskId == taskId, includeProperties: "OrderDetail.Service,OrderDetail.MartyrGrave");

            // Đảm bảo task trả về là một thực thể duy nhất
            var singleTask = task.FirstOrDefault(); // Lấy task đầu tiên (hoặc null nếu không có task nào)

            if (singleTask == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            // Lấy thông tin Account để ánh xạ Fullname
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(singleTask.AccountId);

            // Ánh xạ task sang TaskDtoResponse
            var taskDto = _mapper.Map<TaskDtoResponse>(singleTask);
            taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

            // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
            taskDto.ServiceName = singleTask.OrderDetail?.Service?.ServiceName;
            taskDto.ServiceDescription = singleTask.OrderDetail?.Service?.Description;

            // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
            var martyrGrave = singleTask.OrderDetail?.MartyrGrave;
            if (martyrGrave != null)
            {
                var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
            }
            var taskImages = await _unitOfWork.TaskImageRepository.GetAsync(i => i.TaskId == singleTask.TaskId);
            if(taskImages != null)
            {
                foreach (var image in taskImages)
                {
                    if (image.ImageWorkSpace != null)
                    {
                        var taskImage = new TaskImageDtoResponse
                        {
                            images = image.ImageWorkSpace,
                        };
                        taskDto.TaskImages.Add(taskImage);
                    }
                }
            }

            return taskDto;
        }






        //public async Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask)
        //{
        //    // Kiểm tra xem AccountId có tồn tại không
        //    var account = await _unitOfWork.AccountRepository.GetByIDAsync(newTask.AccountId);
        //    if (account == null)
        //    {
        //        throw new KeyNotFoundException("AccountId does not exist.");
        //    }

        //    // Kiểm tra Role của account phải là Role 3 (Staff)
        //    if (account.RoleId != 3)
        //    {
        //        throw new UnauthorizedAccessException("The account is not authorized to perform this task (not a staff account).");
        //    }

        //    // Kiểm tra xem OrderId có tồn tại không
        //    var order = await _unitOfWork.OrderRepository.GetByIDAsync(newTask.OrderId);
        //    if (order == null)
        //    {
        //        throw new KeyNotFoundException("OrderId does not exist.");
        //    }

        //    // Kiểm tra trạng thái của Order phải là 1 (đã thanh toán)
        //    if (order.Status != 1)
        //    {
        //        throw new InvalidOperationException("Order has not been paid (status must be 1).");
        //    }

        //    // Kiểm tra nếu StartDate của nhiệm vụ (thời điểm hiện tại) lớn hơn EndDate của Order
        //    if (DateTime.Now > order.EndDate)
        //    {
        //        throw new InvalidOperationException("Cannot create a task because the start date is after the order's end date.");
        //    }

        //    // Lấy tất cả các chi tiết của Order để kiểm tra khu vực của tất cả các mộ
        //    var orderDetails = await _unitOfWork.OrderDetailRepository
        //        .GetAsync(od => od.OrderId == newTask.OrderId, includeProperties: "MartyrGrave");

        //    if (orderDetails == null || !orderDetails.Any())
        //    {
        //        throw new InvalidOperationException("Order details are not associated with any martyr grave.");
        //    }

        //    foreach (var detail in orderDetails)
        //    {
        //        var martyrGrave = detail.MartyrGrave;
        //        if (martyrGrave == null)
        //        {
        //            throw new InvalidOperationException("Order details are not associated with any martyr grave.");
        //        }

        //        // Kiểm tra nếu nhân viên chỉ được làm việc trong khu vực của họ
        //        if (account.AreaId != martyrGrave.AreaId)
        //        {
        //            throw new UnauthorizedAccessException("Staff can only work in their assigned area.");
        //        }
        //    }

        //    // Nếu tất cả các mộ đều nằm trong khu vực của nhân viên, tạo Task mới
        //    var taskEntity = new StaffTask
        //    {
        //        AccountId = newTask.AccountId,
        //        OrderId = newTask.OrderId,
        //        NameOfWork = newTask.NameOfWork,
        //        TypeOfWork = newTask.TypeOfWork,
        //        StartDate = DateTime.Now,  // Gán StartDate là thời gian hiện tại
        //        EndDate = order.EndDate,   // Lấy EndDate từ Order
        //        Description = newTask.Description,
        //        Status = 0
        //    };

        //    // Thêm Task vào cơ sở dữ liệu
        //    await _unitOfWork.TaskRepository.AddAsync(taskEntity);
        //    await _unitOfWork.SaveAsync();

        //    return _mapper.Map<TaskDtoResponse>(taskEntity);
        //}


        //public async Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask, int managerId)
        //{
        //    // 1. Kiểm tra xem AccountId (Manager) có tồn tại không từ context của Manager đang đăng nhập
        //    var managerAccount = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
        //    if (managerAccount == null) 
        //    {
        //        throw new KeyNotFoundException("ManagerId does not exist.");
        //    }

        //    // 2. Kiểm tra Role của account phải là Role 2 (Manager)
        //    if (managerAccount.RoleId != 2)
        //    {
        //        throw new UnauthorizedAccessException("The account is not authorized to create this task (not a manager account).");
        //    }

        //    // 3. Kiểm tra xem AccountId của staff có tồn tại không
        //    var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(newTask.AccountId);
        //    if (staffAccount == null)
        //    {
        //        throw new KeyNotFoundException("Staff AccountId does not exist.");
        //    }

        //    // 4. Kiểm tra Role của account phải là Role 3 (Staff)
        //    if (staffAccount.RoleId != 3)
        //    {
        //        throw new UnauthorizedAccessException("The specified account is not a staff account.");
        //    }

        //    // 5. Kiểm tra xem OrderId có tồn tại không
        //    var order = await _unitOfWork.OrderRepository.GetByIDAsync(newTask.OrderId);
        //    if (order == null)
        //    {
        //        throw new KeyNotFoundException("OrderId does not exist.");
        //    }

        //    // 6. Kiểm tra trạng thái của Order phải là 1 (đã thanh toán)
        //    if (order.Status != 1)
        //    {
        //        throw new InvalidOperationException("Order has not been paid (status must be 1).");
        //    }

        //    // 7. Kiểm tra nếu StartDate của nhiệm vụ (thời điểm hiện tại) lớn hơn EndDate của Order
        //    if (DateTime.Now > order.EndDate)
        //    {
        //        throw new InvalidOperationException("Cannot create a task because the start date is after the order's end date.");
        //    }

        //    // 8. Kiểm tra xem OrderDetail có tồn tại không
        //    var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIDAsync(newTask.DetailId);
        //    if (orderDetail == null)
        //    {
        //        throw new KeyNotFoundException("OrderDetailId does not exist.");
        //    }

        //    // 9. Kiểm tra nếu OrderDetail có liên quan đến OrderId được truyền vào
        //    if (orderDetail.OrderId != newTask.OrderId)
        //    {
        //        throw new InvalidOperationException("The specified OrderDetail does not belong to the given Order.");
        //    }

        //    // 10. Lấy thông tin MartyrGrave từ MartyrId
        //    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(orderDetail.MartyrId);
        //    if (martyrGrave == null)
        //    {
        //        throw new KeyNotFoundException("MartyrGrave does not exist.");
        //    }

        //    // 11. Kiểm tra khu vực làm việc của Staff
        //    if (martyrGrave.AreaId != staffAccount.AreaId)
        //    {
        //        throw new UnauthorizedAccessException("Staff can only be assigned tasks in their assigned area.");
        //    }

        //    // 12. Kiểm tra xem Task đã tồn tại với OrderDetail này chưa
        //    if (orderDetail.StaffTask != null)
        //    {
        //        throw new InvalidOperationException("A task has already been created for this OrderDetail.");
        //    }

        //    // 13. Tự động điều chỉnh EndDate của Task không được vượt quá EndDate của Order
        //    DateTime taskEndDate = order.EndDate;  // Mặc định lấy EndDate của Order

        //    // Nếu có EndDate từ phía request và nhỏ hơn hoặc bằng EndDate của Order thì sử dụng
        //    if (newTask.EndDate <= order.EndDate)
        //    {
        //        taskEndDate = newTask.EndDate;
        //    }

        //    // 14. Nếu tất cả điều kiện hợp lệ, tạo Task mới với AccountId của Staff
        //    var taskEntity = new StaffTask
        //    {
        //        AccountId = newTask.AccountId,  // Gán AccountId của staff từ request
        //        OrderId = newTask.OrderId,
        //        DetailId = newTask.DetailId,  // Gán OrderDetailId
        //        Description = newTask.Description,
        //        StartDate = DateTime.Now,  // Gán StartDate là thời gian hiện tại
        //        EndDate = taskEndDate,   // EndDate là giá trị đã điều chỉnh
        //        Status = 1,  // Status ban đầu là 1 (đã bàn giao)
        //        ImagePath1 = newTask.ImagePath1,
        //        ImagePath2 = newTask.ImagePath2,
        //        ImagePath3 = newTask.ImagePath3
        //    };

        //    // 15. Liên kết Task với OrderDetail
        //    orderDetail.StaffTask = taskEntity;

        //    // 16. Thêm Task vào cơ sở dữ liệu
        //    await _unitOfWork.TaskRepository.AddAsync(taskEntity);
        //    await _unitOfWork.SaveAsync();

        //    // 17. Trả về DTO của Task đã tạo
        //    return _mapper.Map<TaskDtoResponse>(taskEntity);
        //}


        //public async Task<List<TaskDtoResponse>> CreateTaskAsync(TaskBatchCreateRequest newTaskBatch, int managerId)
        //{
        //    // 1. Kiểm tra xem AccountId (Manager) có tồn tại không từ context của Manager đang đăng nhập
        //    var managerAccount = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
        //    if (managerAccount == null)
        //    {
        //        throw new KeyNotFoundException("ManagerId does not exist.");
        //    }

        //    // 2. Kiểm tra Role của account phải là Role 2 (Manager)
        //    if (managerAccount.RoleId != 2)
        //    {
        //        throw new UnauthorizedAccessException("The account is not authorized to create this task (not a manager account).");
        //    }

        //    // 3. Kiểm tra xem OrderId có tồn tại không
        //    var order = await _unitOfWork.OrderRepository.GetByIDAsync(newTaskBatch.OrderId);
        //    if (order == null)
        //    {
        //        throw new KeyNotFoundException("OrderId does not exist.");
        //    }

        //    // 4. Kiểm tra trạng thái của Order phải là 1 (đã thanh toán)
        //    if (order.Status != 1)
        //    {
        //        throw new InvalidOperationException("Order has not been paid (status must be 1).");
        //    }

        //    // 5. Kiểm tra nếu StartDate của nhiệm vụ (thời điểm hiện tại) lớn hơn EndDate của Order
        //    if (DateTime.Now > order.EndDate)
        //    {
        //        throw new InvalidOperationException("Cannot create a task because the start date is after the order's end date.");
        //    }

        //    // 6. Lấy tất cả OrderDetail liên quan đến OrderId
        //    var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == newTaskBatch.OrderId);

        //    if (!orderDetails.Any())
        //    {
        //        throw new KeyNotFoundException("No order details found for the given OrderId.");
        //    }

        //    if (orderDetails.Count() != newTaskBatch.TaskDetails.Count)
        //    {
        //        throw new InvalidOperationException("The number of task details does not match the number of order details.");
        //    }

        //    var taskResponses = new List<TaskDtoResponse>();

        //    // 7. Tạo task cho từng OrderDetail
        //    for (int i = 0; i < orderDetails.Count(); i++)
        //    {
        //        var orderDetail = orderDetails.ElementAt(i);
        //        var taskDetail = newTaskBatch.TaskDetails[i];



        //        // Lấy thông tin nhân viên (staff)
        //        var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(taskDetail.AccountId);
        //        if (staffAccount == null || staffAccount.RoleId != 3)
        //        {
        //            throw new KeyNotFoundException("Staff AccountId does not exist or is not a valid staff account.");
        //        }

        //        // Lấy thông tin MartyrGrave từ MartyrId
        //        var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(orderDetail.MartyrId);
        //        if (martyrGrave == null)
        //        {
        //            throw new KeyNotFoundException("MartyrGrave does not exist for OrderDetailId: " + orderDetail.DetailId);
        //        }

        //        // Kiểm tra khu vực làm việc của Staff
        //        if (martyrGrave.AreaId != staffAccount.AreaId)
        //        {
        //            throw new UnauthorizedAccessException("Staff can only be assigned tasks in their assigned area for OrderDetailId: " + orderDetail.DetailId);
        //        }

        //        // Tự động điều chỉnh EndDate của Task không được vượt quá EndDate của Order
        //        DateTime taskEndDate = order.EndDate;

        //        if (taskDetail.EndDate <= order.EndDate)
        //        {
        //            taskEndDate = taskDetail.EndDate;
        //        }

        //        // Tạo task mới
        //        var taskEntity = new StaffTask
        //        {
        //            AccountId = taskDetail.AccountId,
        //            OrderId = newTaskBatch.OrderId,
        //            DetailId = orderDetail.DetailId,
        //            StartDate = DateTime.Now,
        //            EndDate = taskEndDate,
        //            Status = 1  // Trạng thái ban đầu là đã bàn giao
        //        };

        //        // Gán task cho order detail
        //        orderDetail.StaffTask = taskEntity;

        //        await _unitOfWork.TaskRepository.AddAsync(taskEntity);

        //        taskResponses.Add(_mapper.Map<TaskDtoResponse>(taskEntity));
        //    }

        //    await _unitOfWork.SaveAsync();

        //    return taskResponses;
        //}



        public async Task<List<TaskDtoResponse>> CreateTasksAsync(List<TaskDtoRequest> taskDtos)
        {
            try
            {
                var taskResponses = new List<TaskDtoResponse>();

                foreach (var taskDto in taskDtos)
                {
                    // Kiểm tra xem OrderId có tồn tại không
                    var order = await _unitOfWork.OrderRepository.GetByIDAsync(taskDto.OrderId);
                    if (order == null)
                    {
                        throw new KeyNotFoundException("OrderId không tồn tại.");
                    }

                    // Kiểm tra trạng thái của Order, nếu không phải là 1 thì không cho tạo Task
                    if (order.Status != 1)
                    {
                        throw new InvalidOperationException("Order không ở trạng thái hợp lệ để tạo Task.");
                    }

                    // Kiểm tra xem DetailId có tồn tại và thuộc về OrderId không
                    var orderDetail = (await _unitOfWork.OrderDetailRepository.GetAsync(t => t.DetailId == taskDto.DetailId, includeProperties: "Service,MartyrGrave")).FirstOrDefault();
                    if (orderDetail == null || orderDetail.OrderId != taskDto.OrderId)
                    {
                        throw new InvalidOperationException("DetailId không hợp lệ hoặc không thuộc về OrderId được cung cấp.");
                    }

                    // Kiểm tra xem khu vực của MartyrGrave trong OrderDetail
                    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(orderDetail.MartyrId);
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
                        var taskCount = await _unitOfWork.TaskRepository
                            .CountAsync(t => t.AccountId == staff.AccountId); // Lấy công việc đang được chỉ định
                        staffWorkloads[staff.AccountId] = taskCount;
                    }

                    var sortedStaffAccounts = staffAccounts
                        .OrderBy(staff => staffWorkloads[staff.AccountId])
                        .ToList();

                    // Lấy nhân viên có ít công việc nhất
                    var selectedStaff = sortedStaffAccounts.First();

                    // Đặt EndDate của task bằng với ExpectedCompletionDate của Order
                    DateTime taskEndDate = order.ExpectedCompletionDate;

                    // Tạo task mới và gắn Note của Order vào Description
                    var taskEntity = new StaffTask
                    {
                        AccountId = selectedStaff.AccountId,
                        OrderId = taskDto.OrderId,
                        DetailId = taskDto.DetailId,
                        StartDate = DateTime.Now,
                        EndDate = taskEndDate,
                        Description = order.Note,  // Gắn Note của Order vào Description của Task
                        Status = 1  // Trạng thái ban đầu là 'assigned'
                    };

                    // Thêm Task vào cơ sở dữ liệu
                    await _unitOfWork.TaskRepository.AddAsync(taskEntity);

                    // Thêm vào danh sách kết quả
                    taskResponses.Add(_mapper.Map<TaskDtoResponse>(taskEntity));
                    // Tạo thông báo sau khi thanh toán thành công
                    await CreateNotification(
                        "Bạn có một công việc mới",
                        $"Công việc #{orderDetail.Service?.ServiceName} cho mộ {orderDetail.MartyrGrave?.MartyrCode} đã được tạo vào lúc {DateOnly.FromDateTime(taskEntity.StartDate)} và sẽ kết thúc vào ngày {DateOnly.FromDateTime(taskEntity.EndDate)}.",
                        taskEntity.AccountId, "/danhsachdonhang-staff"
                    );
                }

                //await _unitOfWork.SaveAsync();

                return taskResponses;
            }catch(Exception ex)
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






        //public async Task<TaskDtoResponse> UpdateTaskStatusAsync(int taskId, int accountId, int newStatus, List<string>? urlImages = null, string? reason = null)
        //{
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // 1. Kiểm tra TaskId có tồn tại không
        //            var task = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
        //            if (task == null)
        //            {
        //                throw new KeyNotFoundException("TaskId does not exist.");
        //            }

        //            // 2. Kiểm tra AccountId có tồn tại không
        //            var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
        //            if (account == null)
        //            {
        //                throw new KeyNotFoundException("AccountId does not exist.");
        //            }

        //            // 3. Kiểm tra Role của account phải là Role 3 (Staff)
        //            if (account.RoleId != 3)
        //            {
        //                throw new UnauthorizedAccessException("The account is not authorized to perform this task (not a staff account).");
        //            }

        //            // 4. Kiểm tra khu vực làm việc của Staff
        //            var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(
        //            od => od.DetailId == task.DetailId,
        //            includeProperties: "MartyrGrave"
        //            );
        //            var detailEntity = orderDetail.FirstOrDefault();
        //            if (detailEntity == null || detailEntity.MartyrGrave?.AreaId != account.AreaId)
        //            {
        //                throw new UnauthorizedAccessException("Staff can only work in their assigned area.");
        //            }

        //            // 5. Cập nhật trạng thái của Task
        //            if (task.Status == 1)
        //            {
        //                if (newStatus == 2)
        //                {
        //                    task.Status = 2;  // Từ chối task
        //                }
        //                else if (newStatus == 3)
        //                {
        //                    task.Status = 3;  // Nhận task

        //                    // Cập nhật trạng thái của Order sang "đang thực hiện"
        //                    var order = await _unitOfWork.OrderRepository.GetByIDAsync(task.OrderId);
        //                    if (order != null)
        //                    {
        //                        order.Status = 3;  // Order chuyển sang trạng thái "đang thực hiện"
        //                        await _unitOfWork.OrderRepository.UpdateAsync(order);
        //                    }
        //                }
        //                else
        //                {
        //                    throw new InvalidOperationException("You can only update status to 2 (reject) or 3 (in progress) from status 1.");
        //                }
        //            }
        //            else if (task.Status == 3)
        //            {
        //                // Task đang ở trạng thái "đang thực hiện", có thể hoàn thành (lên 4)
        //                if (newStatus == 4)
        //                {
        //                    if (urlImages == null || !urlImages.Any())
        //                    {
        //                        throw new InvalidOperationException("You must provide at least one image when completing the task.");
        //                    }

        //                    // Cập nhật 3 ảnh nếu có
        //                    task.ImagePath1 = urlImages.ElementAtOrDefault(0);  // Ảnh 1
        //                    task.ImagePath2 = urlImages.ElementAtOrDefault(1);  // Ảnh 2 (nếu có)
        //                    task.ImagePath3 = urlImages.ElementAtOrDefault(2);  // Ảnh 3 (nếu có)

        //                    task.Status = 4;  // Hoàn thành task

        //                    // Cập nhật trạng thái của Order sang "hoàn thành"
        //                    var order = await _unitOfWork.OrderRepository.GetByIDAsync(task.OrderId);
        //                    if (order != null)
        //                    {
        //                        order.Status = 4;  // Order chuyển sang trạng thái "hoàn thành"
        //                        await _unitOfWork.OrderRepository.UpdateAsync(order);
        //                    }
        //                }
        //                else
        //                {
        //                    throw new InvalidOperationException("You can only update status to 4 (completed) from status 3.");
        //                }
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException("Invalid status transition.");
        //            }

        //            // 6. Nếu có lý do, cập nhật lý do
        //            if (!string.IsNullOrEmpty(reason))
        //            {
        //                task.Reason = reason;
        //            }

        //            // 7. Lưu thay đổi vào cơ sở dữ liệu
        //            await _unitOfWork.TaskRepository.UpdateAsync(task);
        //            await _unitOfWork.SaveAsync();

        //            // 8. Commit transaction nếu không có lỗi
        //            await transaction.CommitAsync();

        //            return _mapper.Map<TaskDtoResponse>(task);
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback transaction nếu có lỗi
        //            await transaction.RollbackAsync();
        //            throw new Exception($"Failed to update task: {ex.Message}");
        //        }
        //    }
        //}


        //public async Task<TaskDtoResponse> UpdateTaskStatusAsync(int taskId, int newStatus)
        //{
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // 1. Kiểm tra TaskId có tồn tại không
        //            var task = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
        //            if (task == null)
        //            {
        //                throw new KeyNotFoundException("TaskId does not exist.");
        //            }

        //            // 2. Cập nhật trạng thái của Task
        //            if (task.Status == 1)
        //            {
        //                if (newStatus == 2)
        //                {
        //                    task.Status = 2;  // Từ chối task
        //                }
        //                else if (newStatus == 3)
        //                {
        //                    task.Status = 3;  // Nhận task

        //                    //// Lấy tất cả OrderDetail của Order để kiểm tra số lượng Task
        //                    //var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == task.OrderId);
        //                    //bool allTasksForOrderInProgress = true;

        //                    //foreach (var orderDetail in orderDetails)
        //                    //{
        //                    //    // Lấy tất cả các Task của từng OrderDetail
        //                    //    var tasksForDetail = await _unitOfWork.TaskRepository.GetAsync(t => t.DetailId == orderDetail.DetailId);

        //                    //    // Kiểm tra xem OrderDetail đã có Task nào chưa
        //                    //    if (!tasksForDetail.Any())
        //                    //    {
        //                    //        allTasksForOrderInProgress = false;
        //                    //        break;
        //                    //    }

        //                    //    // Kiểm tra nếu tất cả các Task của OrderDetail này có trạng thái là 3
        //                    //    if (tasksForDetail.Any(t => t.Status != 3))
        //                    //    {
        //                    //        allTasksForOrderInProgress = false;
        //                    //        break;
        //                    //    }
        //                    //}

        //                    //if (allTasksForOrderInProgress)
        //                    //{
        //                    //    // Chỉ khi tất cả OrderDetail đã có Task và các Task đều có trạng thái là 3 thì mới cập nhật trạng thái của Order
        //                    //    var order = await _unitOfWork.OrderRepository.GetByIDAsync(task.OrderId);
        //                    //    if (order != null)
        //                    //    {
        //                    //        order.Status = 3;  // Order chuyển sang trạng thái "đang thực hiện"
        //                    //        await _unitOfWork.OrderRepository.UpdateAsync(order);
        //                    //    }
        //                    //}
        //                }
        //                else
        //                {
        //                    throw new InvalidOperationException("You can only update status to 2 (reject) or 3 (in progress) from status 1.");
        //                }
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException("Invalid status transition.");
        //            }

        //            // 3. Lưu thay đổi vào cơ sở dữ liệu
        //            await _unitOfWork.TaskRepository.UpdateAsync(task);
        //            await _unitOfWork.SaveAsync();

        //            // 4. Commit transaction nếu không có lỗi
        //            await transaction.CommitAsync();

        //            return _mapper.Map<TaskDtoResponse>(task);
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback transaction nếu có lỗi
        //            await transaction.RollbackAsync();
        //            throw new Exception($"Failed to update task: {ex.Message}");
        //        }
        //    }
        //}



        public async Task<TaskDtoResponse> UpdateTaskStatusAsync(int taskId, int newStatus)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra TaskId có tồn tại không
                    var task = (await _unitOfWork.TaskRepository.GetAsync(t => t.TaskId == taskId, includeProperties:"OrderDetail.Service,Account")).FirstOrDefault();
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }

                    // 2. Cập nhật trạng thái của Task
                    if (task.Status == 1)
                    {
                        if (newStatus == 2)
                        {
                            task.Status = 2;  // Từ chối task
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
                    await _unitOfWork.TaskRepository.UpdateAsync(task);
                    if (task.Account.AreaId != null)
                    {
                        var manager = (await _unitOfWork.AccountRepository.GetAsync(m => m.RoleId == 2 && m.AreaId == task.Account.AreaId)).FirstOrDefault();
                        if (manager != null)
                        {
                            await CreateNotification(
                    "Một công việc đã bị từ chối bởi nhân viên",
                    $"Công việc {task.OrderDetail?.Service?.ServiceName} đã bị từ chối bởi {task.Account.FullName}. Hãy kiểm tra lại công việc đó",
                    manager.AccountId, $"/danhsachdonhang/{task.DetailId}?managerId={manager.AccountId}"
                    );
                        }
                    }
                    await _unitOfWork.SaveAsync();

                    // 4. Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return _mapper.Map<TaskDtoResponse>(task);
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to update task: {ex.Message}");
                }
            }
        }









        //public async Task<TaskDtoResponse> UpdateTaskImagesAsync(int taskId, TaskImageUpdateDTO imageUpdateDto)
        //{
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // 1. Kiểm tra TaskId có tồn tại không
        //            var task = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
        //            if (task == null)
        //            {
        //                throw new KeyNotFoundException("TaskId does not exist.");
        //            }

        //            // 2. Kiểm tra nếu task có thể cập nhật hình ảnh (task phải ở trạng thái "đang thực hiện")
        //            if (task.Status != 3)
        //            {
        //                throw new InvalidOperationException("Task is not in a state that allows image updates.");
        //            }

        //            // 3. Cập nhật hình ảnh
        //            task.ImagePath1 = imageUpdateDto.UrlImages.ElementAtOrDefault(0);  // Ảnh 1
        //            task.ImagePath2 = imageUpdateDto.UrlImages.ElementAtOrDefault(1);  // Ảnh 2 (nếu có)
        //            task.ImagePath3 = imageUpdateDto.UrlImages.ElementAtOrDefault(2);  // Ảnh 3 (nếu có)

        //            // 4. Cập nhật trạng thái task lên 4
        //            task.Status = 4;  // Task hoàn thành
        //            await _unitOfWork.TaskRepository.UpdateAsync(task);

        //            // 5. Kiểm tra nếu tất cả các task của Order này đều đã hoàn thành
        //            var allTasksForOrder = await _unitOfWork.TaskRepository.GetAsync(t => t.OrderId == task.OrderId);
        //            if (allTasksForOrder.All(t => t.Status == 4)) // Kiểm tra tất cả task có status là 4
        //            {
        //                var order = await _unitOfWork.OrderRepository.GetByIDAsync(task.OrderId);
        //                if (order != null)
        //                {
        //                    // Cập nhật trạng thái của Order sang 4 nếu tất cả các Task đã hoàn thành
        //                    order.Status = 4;  // Order hoàn thành
        //                    await _unitOfWork.OrderRepository.UpdateAsync(order);
        //                }
        //            }

        //            // 6. Lưu thay đổi
        //            await _unitOfWork.SaveAsync();

        //            // 7. Commit transaction nếu không có lỗi
        //            await transaction.CommitAsync();

        //            return _mapper.Map<TaskDtoResponse>(task);
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback transaction nếu có lỗi
        //            await transaction.RollbackAsync();
        //            throw new Exception($"Failed to update task images: {ex.Message}");
        //        }
        //    }
        //}


        public async Task<bool> UpdateTaskImagesAsync(int taskId, TaskImageUpdateDTO imageUpdateDto)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra TaskId có tồn tại không
                    var task = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }

                    // 2. Kiểm tra nếu task có thể cập nhật hình ảnh (task phải ở trạng thái "đang thực hiện")
                    if (task.Status != 3)
                    {
                        throw new InvalidOperationException("Task is not in a state that allows image updates.");
                    }

                    // 3. Cập nhật hình ảnh
                    task.ImageWorkSpace = imageUpdateDto.ImageWorkSpace;  // Ảnh không gian bàn làm việc

                    foreach(var image in imageUpdateDto.UrlImages)
                    {
                        if (image != null)
                        {
                            var imageTask = new TaskImage
                            {
                                TaskId = task.TaskId,
                                ImageWorkSpace = image,
                                CreateAt = DateTime.Now
                            };
                            await _unitOfWork.TaskImageRepository.AddAsync(imageTask);
                        }
                    }


                    // 4. Cập nhật trạng thái task lên 4
                    task.Status = 4;  // Task hoàn thành
                    await _unitOfWork.TaskRepository.UpdateAsync(task);

                    // 5. Kiểm tra nếu tất cả các task của tất cả OrderDetail của Order này đều đã hoàn thành
                    var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == task.OrderId);
                    bool allOrderDetailsHaveCompletedTasks = true;

                    foreach (var orderDetail in orderDetails)
                    {
                        // Lấy tất cả các Task của từng OrderDetail
                        var tasksForDetail = await _unitOfWork.TaskRepository.GetAsync(t => t.DetailId == orderDetail.DetailId);

                        // Nếu một OrderDetail không có Task nào, hoặc có Task chưa hoàn thành, không cập nhật Order
                        if (!tasksForDetail.Any() || tasksForDetail.Any(t => t.Status != 4))
                        {
                            allOrderDetailsHaveCompletedTasks = false;
                            break;
                        }
                    }

                    if (allOrderDetailsHaveCompletedTasks)
                    {
                        // Chỉ khi tất cả các OrderDetail có Task và tất cả Task đó đều hoàn thành thì cập nhật trạng thái Order
                        var order = await _unitOfWork.OrderRepository.GetByIDAsync(task.OrderId);
                        if (order != null)
                        {
                            order.Status = 4;  // Order hoàn thành
                            await _unitOfWork.OrderRepository.UpdateAsync(order);
                            await CreateNotification(
                            "Đơn hàng của bạn đã được hoàn thành",
                            $"Đơn hàng {order.OrderId} đã được hoàn thành. Khách hàng có thể kiểm tra lại và cho phản hồi",
                            order.AccountId, $"/order-detail-cus/{order.OrderId}"
                            );
                        }
                    }


                    // 7. Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to update task images: {ex.Message}");
                }
            }
        }



        public async Task<TaskDtoResponse> ReassignTaskAsync(int detailId, int newAccountId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync()) // Bắt đầu transaction
            {
                try
                {
                    // 1. Kiểm tra xem TaskId có tồn tại không
                    var task = (await _unitOfWork.TaskRepository.GetAsync(t => t.DetailId == detailId, includeProperties:"OrderDetail.Service,OrderDetail.MartyrGrave,Account")).FirstOrDefault();
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }

                    // 2. Kiểm tra trạng thái hiện tại của Task
                    if (task.Status != 2)
                    {
                        throw new InvalidOperationException("Task can only be reassigned if it is in status 2 (rejected).");
                    }

                    // 3. Kiểm tra xem AccountId mới có tồn tại không
                    var newAccount = await _unitOfWork.AccountRepository.GetByIDAsync(newAccountId);
                    if (newAccount == null)
                    {
                        throw new KeyNotFoundException("New AccountId does not exist.");
                    }

                    // 4. Kiểm tra Role của account mới phải là Role 3 (Staff)
                    if (newAccount.RoleId != 3)
                    {
                        throw new UnauthorizedAccessException("The new account is not authorized to perform this task (not a staff account).");
                    }

                    // 5. Kiểm tra `OrderDetail` liên quan đến Task và nạp cả MartyrGrave
                    var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(
                        od => od.DetailId == task.DetailId,
                        includeProperties: "MartyrGrave"
                    );
                    var detailEntity = orderDetail.FirstOrDefault();
                    if (detailEntity == null)
                    {
                        throw new InvalidOperationException("No order detail found for this task.");
                    }

                    // 6. Kiểm tra nếu nhân viên chỉ được làm việc trong khu vực của họ
                    if (detailEntity.MartyrGrave?.AreaId != newAccount.AreaId)
                    {
                        throw new UnauthorizedAccessException("Staff can only work in their assigned area.");
                    }

                    // 7. Cập nhật AccountId mới và Status của task
                    task.AccountId = newAccountId; // Bàn giao task cho Account mới
                    task.Status = 1; // Trạng thái về 1 (đã bàn giao)

                    // 8. Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.TaskRepository.UpdateAsync(task);
                    await CreateNotification(
                    "Một công việc mới đã được giao lại bởi quản lý",
                    $"Công việc {task.OrderDetail?.Service?.ServiceName} đã được giao lại cho nhân viên {task.Account.FullName}. Hãy kiểm tra lại công việc đó",
                    task.AccountId, "/danhsachdonhang-staff"
                    );
                    await _unitOfWork.SaveAsync();

                    // 9. Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return _mapper.Map<TaskDtoResponse>(task);
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to reassign task: {ex.Message}");
                }
            }
        }





        public async Task<bool> DeleteTaskAsync(int taskId)
        {//
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra xem TaskId có tồn tại không
                    var task = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }

                    // 2. Kiểm tra trạng thái của Task
                    if (task.Status != 0)
                    {
                        throw new InvalidOperationException("Only tasks with status 0 (not assigned) can be deleted.");
                    }

                    // 3. Kiểm tra quan hệ với OrderDetail
                    var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIDAsync(task.DetailId);
                    if (orderDetail == null)
                    {
                        throw new InvalidOperationException("The order detail associated with this task does not exist.");
                    }

                    // 4. Nếu task được xóa, có thể cập nhật OrderDetail (nếu cần)
                    //orderDetail.StaffTask = null; // Gỡ liên kết task khỏi OrderDetail

                    // 5. Xóa Task khỏi cơ sở dữ liệu
                    await _unitOfWork.TaskRepository.DeleteAsync(task);
                    await _unitOfWork.SaveAsync();

                    // 6. Lưu thay đổi vào OrderDetail (nếu có thay đổi)
                    await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                    await _unitOfWork.SaveAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    return true; // Task đã xóa thành công
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to delete task: {ex.Message}");
                }
            }
        }

        public async Task<(IEnumerable<TaskDtoResponse> taskList, int totalPage)> GetTasksByMartyrGraveId(int martyrGraveId, int accountId, int pageIndex, int pageSize)
        {
            try
            {
                var taskResponse = new List<TaskDtoResponse>();
                var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(martyrGraveId);
                if (accountId == null || martyrGrave.AccountId == accountId)
                {
                    int totalTask = (await _unitOfWork.TaskRepository.GetAsync(s => s.OrderDetail.MartyrGrave.MartyrId == martyrGraveId)).Count();
                    int totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    var tasks = await _unitOfWork.TaskRepository.GetAsync(t => t.OrderDetail.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "OrderDetail.Service.ServiceCategory,OrderDetail.MartyrGrave,Account", pageIndex: pageIndex, pageSize: pageSize);
                    if (tasks != null)
                    {
                        foreach (var task in tasks)
                        {
                            var taskItem = new TaskDtoResponse
                            {
                                TaskId = task.TaskId,
                                Fullname = task.Account.FullName,
                                StartDate = task.StartDate,
                                EndDate = task.EndDate,
                                Status = task.Status,
                                ServiceName = task.OrderDetail.Service.ServiceName,
                                ServiceDescription = task.OrderDetail.Service.Description,
                                CategoryName = task.OrderDetail.Service.ServiceCategory.CategoryName
                            };
                            taskResponse.Add(taskItem);
                        }
                    }
                    return (taskResponse, totalPage);
                }
                else
                {
                    return (taskResponse, 0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
