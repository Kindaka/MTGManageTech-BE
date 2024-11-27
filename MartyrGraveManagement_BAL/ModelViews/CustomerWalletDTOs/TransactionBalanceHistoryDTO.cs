using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs
{
    public class TransactionBalanceHistoryDTO
    {
        public long TransactionId { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
    }
}
