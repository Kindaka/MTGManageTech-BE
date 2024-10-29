using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }
        public int RowNumber { get; set; }
        public int MartyrNumber { get; set; }
        public int AreaNumber { get; set; }
        public bool Status { get; set; }

        public MartyrGrave? MartyrGraves { get; set; }
    }
}
