using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.TaskDTOs
{
    public class TaskDtoRequest
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "OrderId is required.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Name of the work is required.")]
        [StringLength(100, ErrorMessage = "Name of the work can't be longer than 100 characters.")]
        public string NameOfWork { get; set; }

        [Required(ErrorMessage = "TypeOfWork is required.")]
        public int TypeOfWork { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int Status { get; set; }

        public string? UrlImage { get; set; }
    }
}
