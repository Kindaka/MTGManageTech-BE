using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.OrdersDTOs
{
    public class OrdersDTORequest
    {
        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountId { get; set; }

        public int Status { get; set; }

    }
}
