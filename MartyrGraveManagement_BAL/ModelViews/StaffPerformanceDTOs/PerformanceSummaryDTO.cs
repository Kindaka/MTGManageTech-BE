using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs
{
    public class PerformanceSummaryDTO
    {
        // Thống kê chung
        public int TotalStaff { get; set; }
        public double AverageOverallScore { get; set; }
        public double AverageQualityScore { get; set; }
        public double AverageTimeScore { get; set; }
        public double AverageInteractionScore { get; set; }

        // Phân phối điểm số
        public Dictionary<string, int> PerformanceLevelDistribution { get; set; } // Ví dụ: {"Xuất sắc": 5, "Tốt": 10, ...}

        // Top performers
        public List<WorkPerformanceDTO> TopPerformers { get; set; }

        // Thống kê theo khu vực
        public List<AreaPerformanceDTO> AreaPerformances { get; set; }

        // Các metrics tổng hợp
        public int TotalTasksCompleted { get; set; }
        public double AverageTaskCompletionRate { get; set; }
        public double AverageCustomerRating { get; set; }
        public double AverageAttendanceRate { get; set; }
    }
}
