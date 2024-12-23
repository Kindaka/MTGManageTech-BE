using System.ComponentModel.DataAnnotations;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDetailDTOs
{
    public class ScheduleDetailDtoRequest
    {
        [Required]
        public int TaskId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int ScheduleDetailType { get; set; }
    }
}
