using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.TaskDTOs
{
    public class ReassignTaskRequest
    {
        [Required(ErrorMessage = "NewAccountId is required.")]
        public int NewAccountId { get; set; }
    }
}
