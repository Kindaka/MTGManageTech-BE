using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MLModels
{
    public class StaffPerformanceTrainingData
    {
     // Input Features - Task Performance
    public float TaskCompletionRate { get; set; }
    public float AverageTaskDuration { get; set; }
    public float FailureRate { get; set; }
    public float RejectionRate { get; set; }
    public float OnTimeCompletionRate { get; set; }
    public float ComplexTaskRatio { get; set; }

    // Input Features - Customer Feedback
    public float AverageRating { get; set; }
    public float ResponseRate { get; set; }
    public float AverageResponseTime { get; set; }
    public float CustomerSatisfactionIndex { get; set; }

    // Input Features - Attendance
    public float AttendanceRate { get; set; }
    public float PunctualityRate { get; set; }

    // Output Labels
    public float QualityScore { get; set; }
    public float TimeManagementScore { get; set; }
    public float InteractionScore { get; set; }
    }
}

