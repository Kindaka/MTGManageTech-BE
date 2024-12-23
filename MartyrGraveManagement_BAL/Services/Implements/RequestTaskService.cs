using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class RequestTaskService : IRequestTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public RequestTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RequestTaskDtoResponse>> GetAllTasksAsync()
        {
            var tasks = await _unitOfWork.RequestTaskRepository.GetAsync(includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType");

            if (!tasks.Any())
            {
                throw new KeyNotFoundException("No tasks found.");
            }

            // Lấy thông tin FullName từ Account và ánh xạ sang TaskDtoResponse
            var taskResponses = new List<RequestTaskDtoResponse>();
            foreach (var task in tasks)
            {
                // Lấy thông tin Account để ánh xạ Fullname
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(task.StaffId);

                // Ánh xạ task sang TaskDtoResponse
                var taskDto = _mapper.Map<RequestTaskDtoResponse>(task);
                taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

                // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                taskDto.ServiceName = task.RequestCustomer.RequestType.TypeName;
                taskDto.ServiceDescription = task.RequestCustomer.RequestType.TypeDescription;

                // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                var martyrGrave = task.RequestCustomer?.MartyrGrave;
                if (martyrGrave != null)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                    taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                }

                taskResponses.Add(taskDto);
            }

            return taskResponses;
        }






        public async Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date)
        {
            // Kiểm tra xem AccountId có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            int totalPage = 0;
            int totalTask = 0;
            IEnumerable<RequestTask> tasks = new List<RequestTask>();

            if (Date == DateTime.MinValue)
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(s => s.StaffId == accountId)).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3
                tasks = await _unitOfWork.RequestTaskRepository.GetAsync(
                    t => t.StaffId == accountId,
                    includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
            }
            else
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(s => s.StaffId == accountId && s.StartDate == DateOnly.FromDateTime(Date))).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3 theo ngày
                tasks = await _unitOfWork.RequestTaskRepository.GetAsync(
                    t => t.StaffId == accountId && t.StartDate == DateOnly.FromDateTime(Date),
                    includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType",
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
            var taskResponses = new List<RequestTaskDtoResponse>();
            foreach (var task in tasks)
            {
                var taskDto = _mapper.Map<RequestTaskDtoResponse>(task);
                taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

                // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                taskDto.ServiceName = task.RequestCustomer?.RequestType?.TypeName;
                taskDto.ServiceDescription = task.RequestCustomer?.RequestType?.TypeDescription;

                // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                var martyrGrave = task.RequestCustomer?.MartyrGrave;
                if (martyrGrave != null)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                    taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                }

                taskResponses.Add(taskDto);
            }

            return (taskResponses, totalPage);
        }

        public async Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksNotSchedulingByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date)
        {
            // Kiểm tra xem AccountId có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            int totalPage = 0;
            int totalTask = 0;
            IEnumerable<RequestTask> tasks = new List<RequestTask>();

            if (Date == DateTime.MinValue)
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(s => s.StaffId == accountId && (s.Status == 1))).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3
                tasks = await _unitOfWork.RequestTaskRepository.GetAsync(
                    t => t.StaffId == accountId && (t.Status == 1),
                    includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType",
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
            }
            else
            {
                // Thêm điều kiện lọc `status` 1 hoặc 3
                totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(s => s.StaffId == accountId && s.StartDate == DateOnly.FromDateTime(Date) && (s.Status == 1))).Count();
                totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);

                // Lấy tất cả các `Task` có `status` là 1 hoặc 3 theo ngày
                tasks = await _unitOfWork.RequestTaskRepository.GetAsync(
                    t => t.StaffId == accountId && t.StartDate == DateOnly.FromDateTime(Date) && (t.Status == 1),
                    includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType",
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
            var taskResponses = new List<RequestTaskDtoResponse>();
            foreach (var task in tasks)
            {
                var taskDto = _mapper.Map<RequestTaskDtoResponse>(task);
                taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

                // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                taskDto.ServiceName = task.RequestCustomer?.RequestType?.TypeName;
                taskDto.ServiceDescription = task.RequestCustomer?.RequestType?.TypeDescription;

                // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                var martyrGrave = task.RequestCustomer?.MartyrGrave;
                if (martyrGrave != null)
                {
                    var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                    taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                }

                taskResponses.Add(taskDto);
            }

            return (taskResponses, totalPage);
        }


        public async Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksForManager(int managerId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                // Kiểm tra xem AccountId có tồn tại không
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }
                var taskResponses = new List<RequestTaskDtoResponse>();
                int totalPage = 0;
                int totalTask = 0;
                IEnumerable<RequestTask> tasks = new List<RequestTask>();
                if (Date == DateTime.MinValue)
                {
                    totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(s => s.RequestCustomer.MartyrGrave.AreaId == account.AreaId, includeProperties: "RequestCustomer.MartyrGrave,Account")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    // Lấy tất cả các đơn hàng dựa trên AccountId và bao gồm các chi tiết đơn hàng
                    tasks = await _unitOfWork.RequestTaskRepository.GetAsync(s => s.RequestCustomer.MartyrGrave.AreaId == account.AreaId, includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType,Account",
                    pageIndex: pageIndex, pageSize: pageSize);
                }
                else
                {
                    totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(s => s.RequestCustomer.MartyrGrave.AreaId == account.AreaId && s.StartDate == DateOnly.FromDateTime(Date), includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType")).Count();
                    totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    // Lấy tất cả các đơn hàng dựa trên AccountId và bao gồm các chi tiết đơn hàng
                    tasks = await _unitOfWork.RequestTaskRepository.GetAsync(t => t.RequestCustomer.MartyrGrave.AreaId == account.AreaId && t.StartDate == DateOnly.FromDateTime(Date), includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType,Account",
                    pageIndex: pageIndex, pageSize: pageSize);
                }


                // Lấy danh sách các Task thuộc về account, bao gồm các bảng liên quan


                if (!tasks.Any())
                {
                    return (taskResponses, 0);
                }



                foreach (var task in tasks)
                {
                    var taskDto = _mapper.Map<RequestTaskDtoResponse>(task);
                    taskDto.Fullname = task.Account?.FullName;  // Ánh xạ FullName từ Account

                    // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
                    taskDto.ServiceName = task.RequestCustomer?.RequestType?.TypeName;
                    taskDto.ServiceDescription = task.RequestCustomer?.RequestType?.TypeDescription;

                    // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
                    var martyrGrave = task.RequestCustomer?.MartyrGrave;
                    if (martyrGrave != null)
                    {
                        var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                        taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
                    }

                    taskResponses.Add(taskDto);
                }

                return (taskResponses, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }






        public async Task<RequestTaskDtoResponse> GetTaskByIdAsync(int taskId)
        {
            // Lấy thông tin Task theo taskId, bao gồm các bảng liên quan
            var task = await _unitOfWork.RequestTaskRepository.GetAsync(t => t.RequestTaskId == taskId, includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType,Account");

            // Đảm bảo task trả về là một thực thể duy nhất
            var singleTask = task.FirstOrDefault(); // Lấy task đầu tiên (hoặc null nếu không có task nào)

            if (singleTask == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            // Lấy thông tin Account để ánh xạ Fullname
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(singleTask.StaffId);

            // Ánh xạ task sang TaskDtoResponse
            var taskDto = _mapper.Map<RequestTaskDtoResponse>(singleTask);
            taskDto.Fullname = account?.FullName;  // Ánh xạ FullName từ Account

            // Lấy thông tin từ OrderDetail, Service, và MartyrGrave
            taskDto.ServiceName = singleTask.RequestCustomer?.RequestType?.TypeName;
            taskDto.ServiceDescription = singleTask.RequestCustomer?.RequestType?.TypeDescription;

            // Ghép vị trí mộ từ AreaNumber, RowNumber, và MartyrNumber
            var martyrGrave = singleTask.RequestCustomer?.MartyrGrave;
            if (martyrGrave != null)
            {
                var location = await _unitOfWork.LocationRepository.GetByIDAsync(martyrGrave.LocationId);
                taskDto.GraveLocation = $"K{location.AreaNumber}-R{location.RowNumber}-{location.MartyrNumber}";
            }
            var taskImages = await _unitOfWork.RequestTaskImageRepository.GetAsync(i => i.RequestTaskId == singleTask.RequestTaskId);
            if (taskImages != null)
            {
                foreach (var image in taskImages)
                {
                    if (image.ImageRequestTaskCustomer != null)
                    {
                        var taskImage = new TaskImageDtoResponse
                        {
                            images = image.ImageRequestTaskCustomer,
                        };
                        taskDto.TaskImages.Add(taskImage);
                    }
                }
            }
            var requestMaterial = await _unitOfWork.RequestMaterialRepository.GetAsync(m => m.RequestId == singleTask.RequestId, includeProperties: "Material,RequestCustomer");
            if (requestMaterial != null)
            {
                foreach (var material in requestMaterial)
                {
                    var taskMaterial = new RequestMaterialDtoResponse
                    {
                        RequestMaterialId = material.RequestMaterialId,
                        RequestId = material.RequestId,
                        MaterialId = material.MaterialId,
                        CreatedAt = material.CreatedAt,
                        MaterialName = material?.Material?.MaterialName,
                        Description = material?.Material?.Description,
                        ImagePath = material?.Material?.ImagePath,
                        Price = material?.Material?.Price
                    };
                    taskDto.Materials.Add(taskMaterial);
                };
            }

            return taskDto;
        }



        //public async Task<List<RequestTaskDtoResponse>> CreateTasksAsync(List<TaskDtoRequest> taskDtos)
        //{
        //    try
        //    {
        //        var taskResponses = new List<TaskDtoResponse>();

        //        foreach (var taskDto in taskDtos)
        //        {
        //            // Kiểm tra xem OrderId có tồn tại không
        //            var order = await _unitOfWork.OrderRepository.GetByIDAsync(taskDto.OrderId);
        //            if (order == null)
        //            {
        //                throw new KeyNotFoundException("OrderId không tồn tại.");
        //            }

        //            // Kiểm tra trạng thái của Order, nếu không phải là 1 thì không cho tạo Task
        //            if (order.Status != 1)
        //            {
        //                throw new InvalidOperationException("Order không ở trạng thái hợp lệ để tạo Task.");
        //            }

        //            // Kiểm tra xem DetailId có tồn tại và thuộc về OrderId không
        //            var orderDetail = (await _unitOfWork.OrderDetailRepository.GetAsync(t => t.DetailId == taskDto.DetailId, includeProperties: "Service,MartyrGrave")).FirstOrDefault();
        //            if (orderDetail == null || orderDetail.OrderId != taskDto.OrderId)
        //            {
        //                throw new InvalidOperationException("DetailId không hợp lệ hoặc không thuộc về OrderId được cung cấp.");
        //            }

        //            // Kiểm tra xem khu vực của MartyrGrave trong OrderDetail
        //            var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(orderDetail.MartyrId);
        //            if (martyrGrave == null)
        //            {
        //                throw new InvalidOperationException("MartyrGrave không tồn tại.");
        //            }

        //            // Lấy danh sách nhân viên thuộc cùng khu vực
        //            var staffAccounts = await _unitOfWork.AccountRepository
        //                .FindAsync(a => a.RoleId == 3 && a.AreaId == martyrGrave.AreaId);

        //            if (!staffAccounts.Any())
        //            {
        //                throw new InvalidOperationException("Không có nhân viên nào thuộc khu vực phù hợp để nhận công việc.");
        //            }

        //            // Sắp xếp danh sách nhân viên dựa trên số lượng công việc
        //            var staffWorkloads = new Dictionary<int, int>();
        //            foreach (var staff in staffAccounts)
        //            {
        //                var taskCount = await _unitOfWork.TaskRepository
        //                    .CountAsync(t => t.AccountId == staff.AccountId); // Lấy công việc đang được chỉ định
        //                staffWorkloads[staff.AccountId] = taskCount;
        //            }

        //            var sortedStaffAccounts = staffAccounts
        //                .OrderBy(staff => staffWorkloads[staff.AccountId])
        //                .ToList();

        //            // Lấy nhân viên có ít công việc nhất
        //            var selectedStaff = sortedStaffAccounts.First();

        //            // Đặt EndDate của task bằng với ExpectedCompletionDate của Order
        //            DateTime taskEndDate = order.ExpectedCompletionDate;

        //            // Tạo task mới và gắn Note của Order vào Description
        //            var taskEntity = new StaffTask
        //            {
        //                AccountId = selectedStaff.AccountId,
        //                OrderId = taskDto.OrderId,
        //                DetailId = taskDto.DetailId,
        //                StartDate = DateTime.Now,
        //                EndDate = taskEndDate,
        //                Description = order.Note,  // Gắn Note của Order vào Description của Task
        //                Status = 1  // Trạng thái ban đầu là 'assigned'
        //            };

        //            // Thêm Task vào cơ sở dữ liệu
        //            await _unitOfWork.TaskRepository.AddAsync(taskEntity);

        //            // Thêm vào danh sách kết quả
        //            taskResponses.Add(_mapper.Map<TaskDtoResponse>(taskEntity));
        //            // Tạo thông báo sau khi thanh toán thành công
        //            await CreateNotification(
        //                "Bạn có một công việc mới",
        //                $"Công việc #{orderDetail.Service?.ServiceName} cho mộ {orderDetail.MartyrGrave?.MartyrCode} đã được tạo vào lúc {DateOnly.FromDateTime(taskEntity.StartDate)} và sẽ kết thúc vào ngày {DateOnly.FromDateTime(taskEntity.EndDate)}.",
        //                taskEntity.AccountId, "/danhsachdonhang-staff"
        //            );
        //        }

        //        //await _unitOfWork.SaveAsync();

        //        return taskResponses;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //}


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



        public async Task<RequestTaskDtoResponse> UpdateTaskStatusAsync(int taskId, AssignmentTaskStatusUpdateDTO newStatus, int staffId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra TaskId có tồn tại không
                    var task = (await _unitOfWork.RequestTaskRepository.GetAsync(t => t.RequestTaskId == taskId, includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType,Account")).FirstOrDefault();
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }
                    if (task.StaffId != staffId)
                    {
                        throw new KeyNotFoundException("Không phải là task của bạn.");
                    }

                    // 2. Cập nhật trạng thái của Task
                    if (task.Status == 1)
                    {
                        if (newStatus.Status == 2)
                        {
                            task.Status = 2;  // Từ chối task
                            task.Reason = newStatus.Reason;
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
                    await _unitOfWork.RequestTaskRepository.UpdateAsync(task);
                    if (task.Account.AreaId != null)
                    {
                        var manager = (await _unitOfWork.AccountRepository.GetAsync(m => m.RoleId == 2 && m.AreaId == task.Account.AreaId)).FirstOrDefault();
                        if (manager != null)
                        {
                            await CreateNotification(
                    "Một công việc đã bị từ chối bởi nhân viên",
                    $"Công việc {task.RequestCustomer?.RequestType?.TypeName} đã bị từ chối bởi {task.Account.FullName}. Hãy kiểm tra lại công việc đó",
                    manager.AccountId, ""
                    );
                        }
                    }
                    await _unitOfWork.SaveAsync();

                    // 4. Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return _mapper.Map<RequestTaskDtoResponse>(task);
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to update task: {ex.Message}");
                }
            }
        }


        public async Task<bool> UpdateTaskImagesAsync(int taskId, TaskImageUpdateDTO imageUpdateDto, int staffId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // 1. Kiểm tra TaskId có tồn tại không
                    var task = await _unitOfWork.RequestTaskRepository.GetByIDAsync(taskId);
                    if (task == null)
                    {
                        throw new KeyNotFoundException("TaskId does not exist.");
                    }
                    if (task.StaffId != staffId)
                    {
                        throw new InvalidOperationException("Không phải công việc của bạn.");
                    }

                    // 2. Kiểm tra nếu task có thể cập nhật hình ảnh (task phải ở trạng thái "đang thực hiện")
                    if (task.Status != 3)
                    {
                        throw new InvalidOperationException("Task is not in a state that allows image updates.");
                    }

                    // 3. Cập nhật hình ảnh
                    task.ImageWorkSpace = imageUpdateDto.ImageWorkSpace;  // Ảnh không gian bàn làm việc

                    foreach (var image in imageUpdateDto.UrlImages)
                    {
                        if (image != null)
                        {
                            var imageTask = new RequestTaskImage
                            {
                                RequestTaskId = task.RequestTaskId,
                                ImageRequestTaskCustomer = image,
                                CreateAt = DateTime.Now
                            };
                            await _unitOfWork.RequestTaskImageRepository.AddAsync(imageTask);
                        }
                    }


                    // 4. Cập nhật trạng thái task lên 4
                    task.Status = 4;  // Task hoàn thành
                    await _unitOfWork.RequestTaskRepository.UpdateAsync(task);

                    var requestCustomer = await _unitOfWork.RequestCustomerRepository.GetByIDAsync(task.RequestId);
                    if (requestCustomer == null)
                    {
                        return false;
                    }
                    requestCustomer.Status = 7;
                    await _unitOfWork.RequestCustomerRepository.UpdateAsync(requestCustomer);

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



        public async Task<RequestTaskDtoResponse> ReassignTaskAsync(int taskId, int newAccountId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync()) // Bắt đầu transaction
            {
                try
                {
                    // 1. Kiểm tra xem TaskId có tồn tại không
                    var task = (await _unitOfWork.RequestTaskRepository.GetAsync(t => t.RequestTaskId == taskId, includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType,Account")).FirstOrDefault();
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

                    // 6. Kiểm tra nếu nhân viên chỉ được làm việc trong khu vực của họ
                    if (task.RequestCustomer.MartyrGrave?.AreaId != newAccount.AreaId)
                    {
                        throw new UnauthorizedAccessException("Staff can only work in their assigned area.");
                    }
                    if (task.StaffId == newAccountId)
                    {
                        throw new InvalidOperationException("Nhân viên mới được giao phải khác với người nhân viên cũ.");
                    }

                    // 7. Cập nhật AccountId mới và Status của task
                    task.StaffId = newAccountId; // Bàn giao task cho Account mới
                    task.Status = 1; // Trạng thái về 1 (đã bàn giao)

                    // 8. Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.RequestTaskRepository.UpdateAsync(task);
                    await CreateNotification(
                    "Một công việc mới đã được giao lại bởi quản lý",
                    $"Công việc theo yêu cầu khách hàng {task.RequestCustomer?.RequestType?.TypeName} đã được giao lại cho nhân viên {task.Account.FullName}. Hãy kiểm tra lại công việc đó",
                    task.StaffId, "/danhsachdonhang-staff"
                    );
                    await _unitOfWork.SaveAsync();

                    // 9. Commit transaction nếu không có lỗi
                    await transaction.CommitAsync();

                    return _mapper.Map<RequestTaskDtoResponse>(task);
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception($"Failed to reassign task: {ex.Message}");
                }
            }
        }





        //public async Task<bool> DeleteTaskAsync(int taskId)
        //{//
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // 1. Kiểm tra xem TaskId có tồn tại không
        //            var task = await _unitOfWork.RequestTaskRepository.GetByIDAsync(taskId);
        //            if (task == null)
        //            {
        //                throw new KeyNotFoundException("TaskId does not exist.");
        //            }

        //            // 2. Kiểm tra trạng thái của Task
        //            if (task.Status != 0)
        //            {
        //                throw new InvalidOperationException("Only tasks with status 0 (not assigned) can be deleted.");
        //            }

        //            // 3. Kiểm tra quan hệ với OrderDetail
        //            var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIDAsync(task.DetailId);
        //            if (orderDetail == null)
        //            {
        //                throw new InvalidOperationException("The order detail associated with this task does not exist.");
        //            }

        //            // 4. Nếu task được xóa, có thể cập nhật OrderDetail (nếu cần)
        //            //orderDetail.StaffTask = null; // Gỡ liên kết task khỏi OrderDetail

        //            // 5. Xóa Task khỏi cơ sở dữ liệu
        //            await _unitOfWork.TaskRepository.DeleteAsync(task);
        //            await _unitOfWork.SaveAsync();

        //            // 6. Lưu thay đổi vào OrderDetail (nếu có thay đổi)
        //            await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
        //            await _unitOfWork.SaveAsync();

        //            // Commit transaction
        //            await transaction.CommitAsync();

        //            return true; // Task đã xóa thành công
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback transaction nếu có lỗi
        //            await transaction.RollbackAsync();
        //            throw new Exception($"Failed to delete task: {ex.Message}");
        //        }
        //    }
        //}

        public async Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksByMartyrGraveId(int martyrGraveId, int accountId, int pageIndex, int pageSize)
        {
            try
            {
                var taskResponse = new List<RequestTaskDtoResponse>();
                var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(martyrGraveId);
                if (accountId == null || martyrGrave.AccountId == accountId)
                {
                    int totalTask = (await _unitOfWork.RequestTaskRepository.GetAsync(s => s.RequestCustomer.MartyrGrave.MartyrId == martyrGraveId)).Count();
                    int totalPage = (int)Math.Ceiling(totalTask / (double)pageSize);
                    var tasks = await _unitOfWork.RequestTaskRepository.GetAsync(t => t.RequestCustomer.MartyrGrave.MartyrId == martyrGraveId, includeProperties: "RequestCustomer.MartyrGrave,RequestCustomer.RequestType,Account", pageIndex: pageIndex, pageSize: pageSize);
                    if (tasks != null)
                    {
                        foreach (var task in tasks)
                        {
                            var taskItem = new RequestTaskDtoResponse
                            {
                                RequestTaskId = task.RequestTaskId,
                                Fullname = task.Account.FullName,
                                StartDate = task.StartDate,
                                EndDate = task.EndDate,
                                Status = task.Status,
                                ServiceName = task.RequestCustomer.RequestType.TypeName,
                                ServiceDescription = task.RequestCustomer.RequestType.TypeDescription
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
