using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs
{
    public class CartItemsDTOResponse
    {
        public int CartId { get; set; }
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public int MartyrId { get; set; }
        public int CartQuantity { get; set; }
        public bool Status { get; set; }

    }
}
