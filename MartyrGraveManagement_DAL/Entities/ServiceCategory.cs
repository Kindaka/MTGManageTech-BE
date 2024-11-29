using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class ServiceCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string CategoryName { get; set; }
        public bool Status { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Description { get; set; }
        public string? UrlImageCategory { get; set; }

        public IEnumerable<Service> Services { get; set; }
    }

}
