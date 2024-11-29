using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Material_Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int MaterialId { get; set; }
        public DateTime CreateAt { get; set; }

        public Material Material { get; set; }
        public Service Service { get; set; }
    }
}
