using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDtoRequest;
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
    }
}
