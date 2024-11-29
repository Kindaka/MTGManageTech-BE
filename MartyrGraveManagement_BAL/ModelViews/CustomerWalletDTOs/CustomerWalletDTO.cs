using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs
{
    public class CustomerWalletDTO
    {
        public int WalletId { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal CustomerBalance { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
