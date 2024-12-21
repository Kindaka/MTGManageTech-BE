using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Request_Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestMaterialId { get; set; }
        public int MaterialId { get; set; }
        public int RequestId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Material? Material { get; set; }
        public RequestCustomer? RequestCustomer { get; set; }
    }
}
