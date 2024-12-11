using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AssignmentTaskFeedbackDTOs
{
    public class AssignmentTaskFeedbackDtoRequest
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "DetailId is required.")]
        public int AssignmentTaskId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } // Thêm thuộc tính Rating
    }

}
