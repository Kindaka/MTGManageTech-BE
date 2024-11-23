using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs
{
    public class UpdatePerformanceDTO
    {
        public double QualityMaintenancePoint { get; set; }
        public double TimeCompletePoint { get; set; }
        public double InteractionPoint { get; set; }
    }
}
