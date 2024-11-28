using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs
{
    public class ServiceScheduleDtoRequest
    {
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public int MartyrId { get; set; }
        public int DayOfService { get; set; }
        public string Note { get; set; }
    }
}
