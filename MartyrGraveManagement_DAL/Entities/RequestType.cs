using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class RequestType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypeId { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string TypeName { get; set; }
        public string? TypeDescription { get; set; }
        public bool Status { get; set; }

        public IEnumerable<RequestCustomer>? RequestCustomers { get; set; }
    }
}
