using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public int MartyrId { get; set; }
        public double OrderPrice { get; set; }
        public int Quantity { get; set; }
        public bool Status { get; set; }

        public Order? Order { get; set; }
        public Service? Service { get; set; }
        public MartyrGrave? MartyrGrave { get; set; }
        public Feedback? Feedback { get; set; }
        public StaffTask? StaffTask { get; set; }
    }

}
