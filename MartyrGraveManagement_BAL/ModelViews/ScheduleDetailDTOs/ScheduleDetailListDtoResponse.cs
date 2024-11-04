using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs
{
    public class ScheduleDetailListDtoResponse
    {
        public int ScheduleDetailId { get; set; }
        public DateOnly Date {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Description { get; set; }
        public string ServiceName { get; set; }
        public string MartyrCode { get; set; }
    }
}
