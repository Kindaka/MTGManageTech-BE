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
        public int AccountId { get; set; }
        public int SlotId { get; set; }
        public int? TaskId { get; set; }
        public string? FullName { get; set; }         // Từ Account
        public string? SlotName { get; set; }         // Từ Slot
        public TimeOnly StartTime { get; set; }      // Từ Slot
        public TimeOnly EndTime { get; set; }        // Từ Slot
        public string? ServiceName { get; set; }      // Từ Service trong OrderDetail
        public string? Location { get; set; }         // Dạng "K{AreaNumber} + R{RowNumber} + {MartyrNumber}" từ MartyrGrave
    }
}
