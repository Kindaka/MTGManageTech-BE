using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.JobDTOs
{
    public class JobDtoRequest
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Name of the work is required.")]
        public string NameOfWork { get; set; }

        [Required(ErrorMessage = "TypeOfWork is required.")]
        public int TypeOfWork { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int Status { get; set; }
    }
}
