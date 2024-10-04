using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.OrdersDetailDTOs
{
    public class OrderDetailDtoResponse
    {
        public string? ServiceName {  get; set; }
        public string? MartyrName { get; set; }
        public double ? OrderPrice { get; set; }
    }
}
