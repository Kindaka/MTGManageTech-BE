namespace MartyrGraveManagement_BAL.ModelViews.DashboardDTOs
{
    public class Top3CustomertDtoResponse
    {
        public int AccountId { get; set; }
        public string? FullName { get; set; }
        public DateTime CreateAt { get; set; }
        public bool Status { get; set; }
        public string? EmailAddress { get; set; }
        public string phoneNumber { get; set; }
        public string? Address { get; set; }
        public string? AvatarPath { get; set; }
        public string? DateOfBirth { get; set; }
        public decimal? customerSpending { get; set; } = 0;
    }
}
