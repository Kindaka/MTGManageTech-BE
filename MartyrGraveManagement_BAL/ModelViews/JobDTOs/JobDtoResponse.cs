using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.JobDTOs
{
    public class JobDtoResponse
    {
        public int JobId { get; set; }
        public int AccountId { get; set; }
        public string NameOfWork { get; set; }
        public int TypeOfWork { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
