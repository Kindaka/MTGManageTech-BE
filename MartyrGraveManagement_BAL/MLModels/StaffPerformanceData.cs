using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MLModels
{
    public class StaffPerformanceData
    {
        public double AverageRating { get; set; }
        public double CompletedTasksCount { get; set; }
        public double TaskCompletionRate { get; set; }
        public double AverageResponseTime { get; set; }
        public double AttendanceRate { get; set; }
        public double TaskQualityScore { get; set; }
        public double CustomerSatisfactionScore { get; set; }
        public double TimeManagementScore { get; set; }
        public double Score { get; set; }
    }

    public class StaffPerformancePrediction
    {
        public double PerformanceScore { get; set; }
    }
}

