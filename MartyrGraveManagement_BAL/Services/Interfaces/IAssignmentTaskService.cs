using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDtoRequest;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAssignmentTaskService
    {
        Task<bool> CreateTasksAsync(int serviceScheduleId);
        Task<AssignmentTaskResponse> ReassignTaskAsync(int taskId, int newStaffId);
        Task<AssignmentTaskResponse> UpdateTaskImagesAsync(int taskId, AssignmentTaskImageUpdateDTO imageUpdateDto);

        Task<AssignmentTaskResponse> UpdateTaskStatusAsync(int taskId, AssignmentTaskStatusUpdateDTO updateDto);

        Task<(IEnumerable<AssignmentTaskResponse> taskList, int totalPage)> GetAssignmentTasksByAccountIdAsync(
            int accountId, 
            int pageIndex, 
            int pageSize, 
            DateTime Date);
        Task<(IEnumerable<AssignmentTaskResponse> taskList, int totalPage)> GetAssignmentTasksNotSchedulingByAccountIdAsync(
            int accountId,
            int pageIndex,
            int pageSize,
            DateTime Date);

        Task<(IEnumerable<AssignmentTaskResponse> taskList, int totalPage)> GetAssignmentTasksForManager(
            int managerId, 
            int pageIndex, 
            int pageSize, 
            DateTime Date);

        Task<AssignmentTaskResponse> GetAssignmentTaskByIdAsync(int taskId);
    }
}
