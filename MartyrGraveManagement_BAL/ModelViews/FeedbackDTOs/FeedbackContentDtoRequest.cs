using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.FeedbackDTOs
{
    public class FeedbackContentDtoRequest
    {
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }  
    }
}
