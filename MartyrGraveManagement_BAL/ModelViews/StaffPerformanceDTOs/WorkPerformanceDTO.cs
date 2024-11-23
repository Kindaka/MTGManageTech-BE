using System;

namespace MartyrGraveManagement_BAL.ModelViews.StaffPerformanceDTOs
{
    public class WorkPerformanceDTO
    {
        // Thông tin cơ bản
        public int WorkId { get; set; }
        public int AccountId { get; set; }
        public string AccountFullName { get; set; }
        public string PhoneNumber { get; set; }

        // Thông tin thời gian
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime UploadTime { get; set; }

        // Điểm số chi tiết (thang điểm 100)
        public double QualityMaintenancePoint { get; set; }  // Điểm chất lượng
        public double TimeCompletePoint { get; set; }        // Điểm thời gian
        public double InteractionPoint { get; set; }         // Điểm tương tác
        public double OverallPoint { get; set; }            // Điểm tổng hợp

        // Thông tin chi tiết về metrics
        public PerformanceMetricsDTO Metrics { get; set; }

        // Nhận xét và đánh giá
        public string Description { get; set; }             // Nhận xét chi tiết
        public string PerformanceLevel { get; set; }        // Mức đánh giá (Xuất sắc/Tốt/Khá/...)
        
        public bool Status { get; set; }
    }

    public class PerformanceMetricsDTO
    {
        // Metrics về Task
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int FailedTasks { get; set; }
        public int RejectedTasks { get; set; }
        public int OnTimeTasks { get; set; }

        // Metrics về Feedback
        public int TotalFeedbacks { get; set; }
        public int RespondedFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public double AverageResponseTime { get; set; }  // Tính bằng giờ

        // Metrics về Attendance
        public int TotalWorkDays { get; set; }
        public int PresentDays { get; set; }
        public int PunctualDays { get; set; }

        // Tỷ lệ phần trăm (để hiển thị)
        public double TaskCompletionRate => TotalTasks > 0 
            ? (double)CompletedTasks / TotalTasks * 100 
            : 0;

        public double TaskSuccessRate => TotalTasks > 0 
            ? (1 - (double)FailedTasks / TotalTasks) * 100 
            : 0;

        public double FeedbackResponseRate => TotalFeedbacks > 0 
            ? (double)RespondedFeedbacks / TotalFeedbacks * 100 
            : 0;

        public double AttendanceRate => TotalWorkDays > 0 
            ? (double)PresentDays / TotalWorkDays * 100 
            : 0;
    }
}
