using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDtoResponse>> GetAllTasksAsync();
        Task<(IEnumerable<TaskDtoResponse> taskList, int totalPage)> GetTasksByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date);
        Task<(IEnumerable<TaskDtoResponse> taskList, int totalPage)> GetTasksForManager(int managerId, int pageIndex, int pageSize, DateTime Date);
        Task<TaskDtoResponse> GetTaskByIdAsync(int taskId);
        Task<IEnumerable<TaskDtoResponse>> GetTasksByMartyrGraveId(int martyrGraveId);
        //Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask);
        //Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask, int managerId);
        //Task<List<TaskDtoResponse>> CreateTaskAsync(TaskBatchCreateRequest newTaskBatch, int managerId);

        Task<List<TaskDtoResponse>> CreateTasksAsync(List<TaskDtoRequest> taskDtos);

        //Task<TaskDtoResponse> UpdateTaskStatusAsync(int taskId, int accountId, int newStatus, List<string>? urlImages = null, string? reason = null);
        Task<TaskDtoResponse> UpdateTaskStatusAsync(int taskId, int newStatus);
        Task<TaskDtoResponse> UpdateTaskImagesAsync(int taskId, TaskImageUpdateDTO imageUpdateDto);
        Task<bool> DeleteTaskAsync(int taskId); //status task 0 

        Task<TaskDtoResponse> ReassignTaskAsync(int taskId, int newAccountId);

    }
}


