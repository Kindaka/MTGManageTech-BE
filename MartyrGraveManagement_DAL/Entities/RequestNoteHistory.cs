using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MartyrGraveManagement_DAL.Entities
{
    public class RequestNoteHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoteId { get; set; }
        public int RequestId { get; set; }
        public int AccountId { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Note { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool Status { get; set; }

        public RequestCustomer? RequestCustomer { get; set; }
    }
}
