using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs
{
    public class ScheduleDetailForTaskDtoResponse
    {
        public int ScheduleDetailId { get; set; }
        public string StaffName { get; set; }
        public DateOnly Date { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Description { get; set; }
        public int? TaskId { get; set; }
        public int? AssignmentTaskId { get; set; }
        public string RecurringType { get; set; }
        public string ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
        public string MartyrCode { get; set; }
        public string? ImageWorkSpace { get; set; }
        public List<TaskImageDtoResponse> ImageTaskImages { get; set; } = new List<TaskImageDtoResponse>();
        public int AreaNumber { get; set; }
        public int RowNumber { get; set; }
        public int MartyrNumber { get; set; }
        public int Status { get; set; }
    }
}
