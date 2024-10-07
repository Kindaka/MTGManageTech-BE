using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
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
            var tasks = await _unitOfWork.TaskRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskDtoResponse>>(tasks);
        }

        public async Task<TaskDtoResponse> GetTaskByIdAsync(int taskId)
        {
            var task = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
            return task == null ? null : _mapper.Map<TaskDtoResponse>(task);
        }

        public async Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask)
        {
            // Kiểm tra xem AccountId có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(newTask.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException("AccountId does not exist.");
            }

            // Kiểm tra Role của account phải là Role 3 (Staff)
            if (account.RoleId != 3)
            {
                throw new UnauthorizedAccessException("The account is not authorized to perform this task (not a staff account).");
            }

            // Kiểm tra xem OrderId có tồn tại không
            var order = await _unitOfWork.OrderRepository.GetByIDAsync(newTask.OrderId);
            if (order == null)
            {
                throw new KeyNotFoundException("OrderId does not exist.");
            }

            // Kiểm tra trạng thái của Order phải là 1 (đã thanh toán)
            if (order.Status != 1)
            {
                throw new InvalidOperationException("Order has not been paid (status must be 1).");
            }

            // Kiểm tra nếu StartDate của nhiệm vụ (thời điểm hiện tại) lớn hơn EndDate của Order
            if (DateTime.Now > order.EndDate)
            {
                throw new InvalidOperationException("Cannot create a task because the start date is after the order's end date.");
            }

            // Lấy tất cả các chi tiết của Order để kiểm tra khu vực của tất cả các mộ
            var orderDetails = await _unitOfWork.OrderDetailRepository
                .GetAsync(od => od.OrderId == newTask.OrderId, includeProperties: "MartyrGrave");

            if (orderDetails == null || !orderDetails.Any())
            {
                throw new InvalidOperationException("Order details are not associated with any martyr grave.");
            }

            foreach (var detail in orderDetails)
            {
                var martyrGrave = detail.MartyrGrave;
                if (martyrGrave == null)
                {
                    throw new InvalidOperationException("Order details are not associated with any martyr grave.");
                }

                // Kiểm tra nếu nhân viên chỉ được làm việc trong khu vực của họ
                if (account.AreaId != martyrGrave.AreaId)
                {
                    throw new UnauthorizedAccessException("Staff can only work in their assigned area.");
                }
            }

            // Nếu tất cả các mộ đều nằm trong khu vực của nhân viên, tạo Task mới
            var taskEntity = new StaffTask
            {
                AccountId = newTask.AccountId,
                OrderId = newTask.OrderId,
                NameOfWork = newTask.NameOfWork,
                TypeOfWork = newTask.TypeOfWork,
                StartDate = DateTime.Now,  // Gán StartDate là thời gian hiện tại
                EndDate = order.EndDate,   // Lấy EndDate từ Order
                Description = newTask.Description,
                Status = newTask.Status
            };

            // Thêm Task vào cơ sở dữ liệu
            await _unitOfWork.TaskRepository.AddAsync(taskEntity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<TaskDtoResponse>(taskEntity);
        }




        public async Task<bool> UpdateTaskAsync(int taskId, TaskDtoRequest updatedTask)
        {
            // Kiểm tra xem TaskId có tồn tại không
            var taskEntity = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
            if (taskEntity == null)
            {
                return false;
            }

            // Kiểm tra xem AccountId có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(updatedTask.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException("AccountId does not exist.");
            }

            // Kiểm tra xem OrderId có tồn tại không
            var order = await _unitOfWork.OrderRepository.GetByIDAsync(updatedTask.OrderId);
            if (order == null)
            {
                throw new KeyNotFoundException("OrderId does not exist.");
            }

            // Cập nhật Task nếu tồn tại
            _mapper.Map(updatedTask, taskEntity);
            await _unitOfWork.TaskRepository.UpdateAsync(taskEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var taskEntity = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
            if (taskEntity == null)
            {
                return false;
            }

            await _unitOfWork.TaskRepository.DeleteAsync(taskEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
