using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs
{
       public class AssignmentTaskStatusUpdateDTO
    {
        [Required(ErrorMessage = "Status is required")]
        public int Status { get; set; }
        [Required(ErrorMessage = "Reason is required")]
        public string? Reason { get; set; }
    }
}
