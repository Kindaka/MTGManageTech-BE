using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs
{
    public class ScheduleDTOResponse
    {
        public int ScheduleId { get; set; }
        public string ManagerName { get; set; }
        public int SlotId { get; set; }
        public string? SlotName { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }      // Từ Slot
        public TimeOnly EndTime { get; set; }        // Từ Slot
    }
}
