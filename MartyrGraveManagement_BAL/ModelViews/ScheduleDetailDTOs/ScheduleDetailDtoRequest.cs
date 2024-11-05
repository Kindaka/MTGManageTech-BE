using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs
{
    public class ScheduleDetailDtoRequest
    {
        public int TaskId { get; set; }
        public int SlotId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }
}
