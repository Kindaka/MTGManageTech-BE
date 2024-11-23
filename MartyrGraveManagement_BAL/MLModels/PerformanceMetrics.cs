using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MLModels
{
    public class PerformanceMetrics
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int FailedTasks { get; set; }
        public int RejectedTasks { get; set; }
        public int OnTimeTasks { get; set; }
        public int ComplexTasks { get; set; }

        public int TotalFeedbacks { get; set; }
        public int RespondedFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public double AverageResponseTime { get; set; }

        public int TotalWorkDays { get; set; }
        public int PresentDays { get; set; }
        public int PunctualDays { get; set; }
    }
}
