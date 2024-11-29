using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs
{
    public class CartItemGetByGuestDTOResponse
    {
        public int ServiceId { get; set; }
        public string MartyrCode { get; set; }
        public int MartyrId { get; set; }

        public ServiceDtoResponse ServiceView { get; set; } = new ServiceDtoResponse();
    }
}
