using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs
{
    public class FeedbackResponseDtoRequest
    {
        [Required(ErrorMessage = "FeedbackId is required.")]
        public int FeedbackId { get; set; }

        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string ResponseContent { get; set; }
    }
}
