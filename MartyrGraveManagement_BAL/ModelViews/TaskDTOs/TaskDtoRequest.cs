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

        [Required(ErrorMessage = "OrderDetailId is required.")]
        public int DetailId { get; set; }


    }
}
