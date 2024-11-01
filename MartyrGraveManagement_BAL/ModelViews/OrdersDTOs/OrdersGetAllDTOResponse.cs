using MartyrGraveManagement_BAL.ModelViews.OrdersDetailDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.OrdersDTOs
{
    public class OrdersGetAllDTOResponse
    {
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public string? Note { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }

        public List<OrderDetailDtoResponse> OrderDetails { get; set; } = new List<OrderDetailDtoResponse>();

    }
}
