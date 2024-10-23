using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string RoleName { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Description { get; set; }
        public bool Status { get; set; }

        public IEnumerable<Account>? Accounts { get; set; }
    }
}
