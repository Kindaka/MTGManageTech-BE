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
        public int ScheduleDetailType { get; set; } //1 là dịch vụ bình thường, 2 là dịch vụ định kì, 3 là yêu cầu thân nhân (request), 4 là báo cáo mộ (quay video)
    }
}
