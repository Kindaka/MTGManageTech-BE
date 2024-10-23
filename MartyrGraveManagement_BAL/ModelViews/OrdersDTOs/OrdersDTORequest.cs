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
        List<CartItemCustomer>? CartItems { get; set; } = new List<CartItemCustomer>();
    }
}
