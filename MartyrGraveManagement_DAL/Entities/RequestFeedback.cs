using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_DAL.Entities
{
    public class RequestFeedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }
        public int CustomerId { get; set; }
        public int RequestId { get; set; }
        public int? StaffId { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Content { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? ResponseContent { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }

        public RequestCustomer? RequestCustomer { get; set; }
    }
}
