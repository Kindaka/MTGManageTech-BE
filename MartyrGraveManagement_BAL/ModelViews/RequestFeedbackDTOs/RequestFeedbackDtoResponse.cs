namespace MartyrGraveManagement_BAL.ModelViews.RequestFeedbackDTOs
{
    public class RequestFeedbackDtoResponse
    {
        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AvatarPath { get; set; }
        public int RequestId { get; set; }
        public int StaffId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }
        public string? ResponseContent { get; set; }
        public int Rating { get; set; }
        public string FullNameStaff { get; set; }
    }
}
