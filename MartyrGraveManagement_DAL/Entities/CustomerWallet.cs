using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class CustomerWallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WalletId { get; set; }
        public int CustomerId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CustomerBalance { get; set; }
        public DateTime UpdateAt { get; set; }

        public Account? Account { get; set; }
    }
}
