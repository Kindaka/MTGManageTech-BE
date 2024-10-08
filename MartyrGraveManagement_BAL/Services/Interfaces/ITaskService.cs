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
        Task<IEnumerable<TaskDtoResponse>> GetTasksByAccountIdAsync(int accountId);
        Task<TaskDtoResponse> GetTaskByIdAsync(int taskId);
        Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask);
        Task<TaskDtoResponse> UpdateTaskStatusAsync(int taskId, int accountId, int newStatus, string? urlImage = null, string? reason = null);

        Task<bool> DeleteTaskAsync(int taskId); //status task 0 

        Task<TaskDtoResponse> ReassignTaskAsync(int taskId, int newAccountId);

    }
}


