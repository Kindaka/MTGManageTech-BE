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

        //public async Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask)
        //{
        //    // Kiểm tra xem AccountId có tồn tại không
        //    var account = await _unitOfWork.AccountRepository.GetByIDAsync(newTask.AccountId);
        //    if (account == null)
        //    {
        //        throw new KeyNotFoundException("AccountId does not exist.");
        //    }

        //    // Kiểm tra xem OrderId có tồn tại không
        //    var order = await _unitOfWork.OrderRepository.GetByIDAsync(newTask.OrderId);
        //    if (order == null)
        //    {
        //        throw new KeyNotFoundException("OrderId does not exist.");
        //    }

        //    var taskEntity = _mapper.Map<StaffTask>(newTask);
        //    await _unitOfWork.TaskRepository.AddAsync(taskEntity);
        //    await _unitOfWork.SaveAsync();
        //    return _mapper.Map<TaskDtoResponse>(taskEntity);
        //}

        //public async Task<bool> UpdateTaskAsync(int taskId, TaskDtoRequest updatedTask)
        //{
        //    // Kiểm tra xem TaskId có tồn tại không
        //    var taskEntity = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
        //    if (taskEntity == null)
        //    {
        //        return false;
        //    }

        //    // Kiểm tra xem AccountId có tồn tại không
        //    var account = await _unitOfWork.AccountRepository.GetByIDAsync(updatedTask.AccountId);
        //    if (account == null)
        //    {
        //        throw new KeyNotFoundException("AccountId does not exist.");
        //    }

        //    // Kiểm tra xem OrderId có tồn tại không
        //    var order = await _unitOfWork.OrderRepository.GetByIDAsync(updatedTask.OrderId);
        //    if (order == null)
        //    {
        //        throw new KeyNotFoundException("OrderId does not exist.");
        //    }

        //    // Cập nhật Task nếu tồn tại
        //    _mapper.Map(updatedTask, taskEntity);
        //    await _unitOfWork.TaskRepository.UpdateAsync(taskEntity);
        //    await _unitOfWork.SaveAsync();
        //    return true;
        //}

        //public async Task<bool> DeleteTaskAsync(int taskId)
        //{
        //    var taskEntity = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
        //    if (taskEntity == null)
        //    {
        //        return false;
        //    }

        //    await _unitOfWork.TaskRepository.DeleteAsync(taskEntity);
        //    await _unitOfWork.SaveAsync();
        //    return true;
        //}
    }
}
