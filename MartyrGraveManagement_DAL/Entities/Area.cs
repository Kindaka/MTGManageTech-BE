using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Area
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }

        public IEnumerable<MartyrGrave>? MartyrGraves { get; set; }
        //public IEnumerable<Account>? Accounts { get; set; }
    }
}
