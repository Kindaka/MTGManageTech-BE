﻿using MartyrGraveManagement_BAL.ModelViews.StaffDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.OrdersDetailDTOs
{
    public class OrderDetailDtoResponse
    {
        public long OrderId { get; set; }
        public int DetailId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public string? Note { get; set; }
        public int orderStatus { get; set; }
        public string? ServiceName {  get; set; }
        public string? MartyrName { get; set; }
        public double ? OrderPrice { get; set; }

        public int StatusTask { get; set; }
        public string? ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; }
        public string? ImagePath3 { get; set; }


        public List<StaffDtoResponse>? Staffs { get; set; } = new List<StaffDtoResponse>();
    }
}
