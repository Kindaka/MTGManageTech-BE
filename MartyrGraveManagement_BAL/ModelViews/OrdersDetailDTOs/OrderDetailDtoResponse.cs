using MartyrGraveManagement_BAL.ModelViews.StaffDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.OrdersDetailDTOs
{
    public class OrderDetailDtoResponse
    {
        public int DetailId { get; set; }
        public string? ServiceName {  get; set; }
        public string? MartyrName { get; set; }
        public double ? OrderPrice { get; set; }

        public int StatusTask { get; set; } = 0;


        public List<StaffDtoResponse>? Staffs { get; set; } = new List<StaffDtoResponse>();
    }
}
