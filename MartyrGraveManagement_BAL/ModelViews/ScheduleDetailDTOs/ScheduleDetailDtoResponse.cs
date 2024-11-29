using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs
{
    public class ScheduleDetailDtoResponse
    {
        public int ScheduleDetailId { get; set; }
        public string StaffName { get; set; }
        public DateOnly Date { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }
        public int? TaskId { get; set; }
        public string ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
        public string MartyrCode { get; set; }
        public string? ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; }
        public string? ImagePath3 { get; set; }
        public int AreaNumber { get; set; }
        public int RowNumber { get; set; }
        public int MartyrNumber { get; set; }
        public int Status { get; set; }
    }
}
