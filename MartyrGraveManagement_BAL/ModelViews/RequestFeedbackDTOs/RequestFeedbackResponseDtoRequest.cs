using System.ComponentModel.DataAnnotations;

namespace MartyrGraveManagement_BAL.ModelViews.RequestFeedbackDTOs
{
    public class RequestFeedbackResponseDtoRequest
    {
        public int FeedbackId { get; set; }
        public int StaffId { get; set; }
        [Required]
        public string? ResponseContent { get; set; }
    }
}
