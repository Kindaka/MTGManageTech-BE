using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.TaskDTOs
{
    public class TaskDetailRequest
    {
        public int AccountId { get; set; }
        public DateTime EndDate { get; set; }
    }
}
