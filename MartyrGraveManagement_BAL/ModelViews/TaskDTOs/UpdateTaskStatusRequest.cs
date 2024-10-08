using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.TaskDTOs
{
    public class UpdateTaskStatusRequest
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Range(2, 4, ErrorMessage = "Status must be between 2 and 4.")]
        public int Status { get; set; }

        public string? UrlImage { get; set; }
        public string? Reason { get; set; }
    }

}
