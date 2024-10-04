using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public int MartyrId { get; set; }
        public bool Status { get; set; }

        public Account? Account { get; set; }
        public Service? Service { get; set; }
        public MartyrGrave? MartyrGrave { get; set; }
    }

}
