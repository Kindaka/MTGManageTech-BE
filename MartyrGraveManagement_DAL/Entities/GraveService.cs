using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class GraveService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GraveServiceId { get; set; }
        public int MartyrId { get; set; }
        public int ServiceId { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public MartyrGrave? MartyrGrave { get; set; }
        public Service? Service { get; set; }
    }
}
