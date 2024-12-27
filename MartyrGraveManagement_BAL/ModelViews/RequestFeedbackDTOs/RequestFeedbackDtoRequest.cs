using System.ComponentModel.DataAnnotations;

namespace MartyrGraveManagement_BAL.ModelViews.RequestFeedbackDTOs
{
    public class RequestFeedbackDtoRequest
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "DetailId is required.")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } // Thêm thuộc tính Rating
    }
}
