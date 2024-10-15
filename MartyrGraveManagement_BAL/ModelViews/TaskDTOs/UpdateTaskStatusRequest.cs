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

        public string? UrlImage1 { get; set; }  // Thêm UrlImage1
        public string? UrlImage2 { get; set; }  // Thêm UrlImage2
        public string? UrlImage3 { get; set; }  // Thêm UrlImage3
        public string? Reason { get; set; }
    }

}
