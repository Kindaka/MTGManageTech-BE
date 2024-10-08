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
        public int OrderId { get; set; }
        public string NameOfWork { get; set; }
        public int TypeOfWork { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string? UrlImage { get; set; }
        public string? Reason { get; set; }
    }
}
