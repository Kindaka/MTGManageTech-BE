using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDtoRequest
{
    public class AssigmentTaskDtoRequest
    {
        [Required(ErrorMessage = "SeriveScheduleId is required.")]
        public int ServiceScheduleId { get; set; }
    }
}
