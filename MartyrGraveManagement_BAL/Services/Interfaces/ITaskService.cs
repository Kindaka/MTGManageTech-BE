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
        Task<TaskDtoResponse> GetTaskByIdAsync(int taskId);
        //Task<TaskDtoResponse> CreateTaskAsync(TaskDtoRequest newTask);
        //Task<bool> UpdateTaskAsync(int taskId, TaskDtoRequest updatedTask);
        //Task<bool> DeleteTaskAsync(int taskId);

    }
}
