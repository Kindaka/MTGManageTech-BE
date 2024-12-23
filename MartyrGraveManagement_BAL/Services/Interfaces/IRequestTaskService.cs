using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestTaskDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IRequestTaskService
    {
        Task<IEnumerable<RequestTaskDtoResponse>> GetAllTasksAsync();
        Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date);
        Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksNotSchedulingByAccountIdAsync(int accountId, int pageIndex, int pageSize, DateTime Date);
        Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksForManager(int managerId, int pageIndex, int pageSize, DateTime Date);
        Task<RequestTaskDtoResponse> GetTaskByIdAsync(int taskId);
        Task<(IEnumerable<RequestTaskDtoResponse> taskList, int totalPage)> GetTasksByMartyrGraveId(int martyrGraveId, int accountId, int pageIndex, int pageSize);

        //Task<List<TaskDtoResponse>> CreateTasksAsync(List<TaskDtoRequest> taskDtos);
        Task<RequestTaskDtoResponse> UpdateTaskStatusAsync(int taskId, AssignmentTaskStatusUpdateDTO newStatus, int staffId);
        Task<bool> UpdateTaskImagesAsync(int taskId, TaskImageUpdateDTO imageUpdateDto, int staffId);
        //Task<bool> DeleteTaskAsync(int taskId); //status task 0 

        Task<RequestTaskDtoResponse> ReassignTaskAsync(int detailId, int newAccountId);
    }
}
