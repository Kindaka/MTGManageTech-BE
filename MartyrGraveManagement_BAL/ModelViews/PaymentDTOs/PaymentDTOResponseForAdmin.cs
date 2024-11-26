using System;
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
        public string BankCode { get; set; }
        public string CardType { get; set; }
        public DateTime PayDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public long OrderId { get; set; }
        public int Status { get; set; }
    }
}
