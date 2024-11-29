using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }
        public long OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string BankCode { get; set; }
        public string? BankTransactionNo { get; set; }
        public string CardType { get; set; }
        public string PaymentInfo { get; set; }
        public DateTime PayDate { get; set; }
        public string TransactionNo { get; set; }
        public int TransactionStatus { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaymentAmount { get; set; }

        public Order Order { get; set; }
    }

}
