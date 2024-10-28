using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs
{
    public class CartItemGetByCustomerDTOResponse
    {
        public int CartId { get; set; }
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public string MartyrCode { get; set; }
<<<<<<< HEAD
        public int MarrtyrId { get; set; }
=======
        public int MartyrId { get; set; }
>>>>>>> f78b91f0778e4078ce0d6a62f407f6e06997ba2d
        public bool Status { get; set; }

        public ServiceDtoResponse ServiceView { get; set; } = new ServiceDtoResponse();
    }
}
