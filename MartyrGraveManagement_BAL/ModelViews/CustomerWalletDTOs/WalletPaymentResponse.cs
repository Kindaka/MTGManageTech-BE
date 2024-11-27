using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs
{
    public class WalletPaymentResponse
    {
        public string PaymentUrl { get; set; }
        public string Message { get; set; }
    }
}
