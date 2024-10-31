using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.OrdersDTOs
{
    public class OrdersDTORequest
    {
        [Required]
        public DateTime ExpectedCompletionDate { get; set; }
        public string? Note { get; set; }
    }
}
