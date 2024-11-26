using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class TransactionBalanceHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TransactionId { get; set; }
        public int CustomerId { get; set; }
        public string? TransactionType { get; set; } // E.g., "Deposit", "Withdrawal", "Payment", "Refund"

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BalanceAfterTransaction { get; set; }

        public Account? Account { get; set; } 
    }
}
