using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedCompletionDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Note { get; set; }
        public string? ImagePath { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? ResponseContent { get; set; }
        public Account? Account { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
        public IEnumerable<Payment>? Payments { get; set; }
    }

}
