using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.PaymentDTOs
{
    public class PaymentDTORequest
    {
        public string? vnp_Amount { get; set; }
        public string? vnp_BankCode { get; set; }
        public string? vnp_BankTranNo { get; set; }
        public string? vnp_CardType { get; set; }
        public string? vnp_OrderInfo { get; set; }
        public string? vnp_PayDate { get; set; }
        public string? vnp_ResponseCode { get; set; }
        public string? vnp_TmnCode { get; set; }
        public string? vnp_TransactionNo { get; set; }
        public string? vnp_TransactionStatus { get; set; }
        public string? vnp_TxnRef { get; set; }
        public string? vnp_SecureHash { get; set; }


        //momo

        public string? orderId { get; set; }
        public string? orderInfo { get; set; }
        public string? requestId { get; set; }
        public string? amount { get; set; }
        public string? resultCode { get; set; }
        public string? partnerCode { get; set; }
        public string? orderType { get; set; }
        public string? transId { get; set; }
        public string? message { get; set; }
        public string? payType { get; set; }
        public string? responseTime { get; set; }
        public string? extraData { get; set; }
        public string? signature { get; set; }
    }
}
