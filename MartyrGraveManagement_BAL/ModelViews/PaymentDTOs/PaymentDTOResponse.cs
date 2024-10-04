using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.PaymentDTOs
{
    public class PaymentDTOResponse
    {
        public string PaymentMethod { get; set; }
        public string? BankCode { get; set; }
        public string? BankTranNo { get; set; }
        public string? CardType { get; set; }
        public string? PaymentInfo { get; set; }
        public DateTime PayDate { get; set; }
        public string? TransactionNo { get; set; }
        public int TransactionStatus { get; set; }
        public double PaymentAmount { get; set; }
        public int OrderId { get; set; }
    }
}
