namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MaintenanceHistoryDtoResponse
    {
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public DateOnly EndDate { get; set; }
        public string? ServiceName { get; set; }
        public string? MartyrCode { get; set; }
        public string? Description { get; set; }
        public string? StaffName { get; set; }
        public string? StaffPhone { get; set; }
        public int Status { get; set; }
    }
}
