using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.TaskDTOs
{
    public class TaskBatchCreateRequest
    {
        public int OrderId { get; set; }
        public List<TaskDetailRequest> TaskDetails { get; set; } = new List<TaskDetailRequest>();
    }
}
