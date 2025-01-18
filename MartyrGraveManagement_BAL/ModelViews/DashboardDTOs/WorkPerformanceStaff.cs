namespace MartyrGraveManagement_BAL.ModelViews.DashboardDTOs
{
    public class WorkPerformanceStaff
    {
        public int totalTask { get; set; } = 0;
        public int totalAssignmentTask { get; set; } = 0;
        public int totalRequestTask { get; set; } = 0;
        public int totalWorkStaff { get; set; } = 0;

        public int totalFinishTask { get; set; } = 0;
        public int totalFinishAssignmentTask { get; set; } = 0;
        public int totalFinishRequestTask { get; set; } = 0;

        public int totalFailTask { get; set; } = 0;
        public int totalFailAssignmentTask { get; set; } = 0;
        public int totalFailRequestTask { get; set; } = 0;



        public decimal averageAllFeedbackRate { get; set; } = 0;

        public string? workPerformance { get; set; } = "Not Avaiable";
        public string? workQuality { get; set; } = "Not Available";
    }
}
