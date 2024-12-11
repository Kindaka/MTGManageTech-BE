using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class AssignmentTask_Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentTaskFeedbackId { get; set; }
        public int CustomerId { get; set; }
        public int AssignmentTaskId { get; set; }
        public int? StaffId { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Content { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? ResponseContent { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }

        public AssignmentTask? AssignmentTask { get; set; }
    }
}
