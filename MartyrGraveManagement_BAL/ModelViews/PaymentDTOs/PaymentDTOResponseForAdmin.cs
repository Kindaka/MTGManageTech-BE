﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.PaymentDTOs
{
    public class PaymentDTOResponseForAdmin
    {
        public string? CustomerName { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PayDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public int OrderId { get; set; }
        public int Status { get; set; }
    }
}
