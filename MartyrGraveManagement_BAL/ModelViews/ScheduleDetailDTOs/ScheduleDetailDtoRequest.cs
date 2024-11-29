using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs
{
    public class ScheduleDetailDtoRequest
    {
        [Required]
        public int TaskId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }
}
