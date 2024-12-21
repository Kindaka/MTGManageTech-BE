using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_DAL.Entities
{
    public class RequestTaskImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestTaskImageId { get; set; }
        public int RequestTaskId { get; set; }
        public string? ImageRequestTaskCustomer { get; set; }
        public DateTime CreateAt { get; set; }

        public RequestTask? RequestTask { get; set; }
    }
}
