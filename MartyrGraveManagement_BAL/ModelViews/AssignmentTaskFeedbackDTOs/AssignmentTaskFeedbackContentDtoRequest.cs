using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AssignmentTaskFeedbackDTOs
{
    public class AssignmentTaskFeedbackContentDtoRequest
    {
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }  
    }
}
