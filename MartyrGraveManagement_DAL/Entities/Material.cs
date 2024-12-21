using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaterialId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string MaterialName { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public bool Status { get; set; }

        public IEnumerable<Material_Service> Material_Services { get; set; }
        public IEnumerable<Request_Material> RequestMaterials { get; set; }

    }
}
