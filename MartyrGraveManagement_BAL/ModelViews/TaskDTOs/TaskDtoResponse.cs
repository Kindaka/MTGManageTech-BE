using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.TaskDTOs
{
    public class TaskDtoResponse
    {
        public int TaskId { get; set; }
        public int AccountId { get; set; }
        public string? Fullname { get; set; }
        public long OrderId { get; set; }
        public int DetailId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string? ImageWorkSpace { get; set; }
        public List<string> TaskImages { get; set; } = new List<string>();
        public string? Reason { get; set; }


        // Các thuộc tính mới
        public string? ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
        public string? ServiceImage { get; set; }
        public string? CategoryName { get; set; }
        public string? GraveLocation { get; set; }
    }
}
