using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaterialId { get; set; }
        public int ServiceId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string MaterialName { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public double Price { get; set; }

        public Service? Service { get; set; }

    }
}
