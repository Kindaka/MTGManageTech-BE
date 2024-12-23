namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs
{
    public class ScheduleDetailListDtoResponse
    {
        public int ScheduleDetailId { get; set; }
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
        public string? ServiceName { get; set; }
        public string? MartyrCode { get; set; }
        public int Status { get; set; }
    }
}
