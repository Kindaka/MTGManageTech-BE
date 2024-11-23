using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs
{
    public class AreaPerformanceDTO
    {
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public int StaffCount { get; set; }
        public double AverageOverallScore { get; set; }
        public List<WorkPerformanceDTO> StaffPerformances { get; set; }
    }
}
