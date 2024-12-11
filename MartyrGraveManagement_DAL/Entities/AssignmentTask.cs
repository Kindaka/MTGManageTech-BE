using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class AssignmentTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentTaskId { get; set; }
        public int ServiceScheduleId { get; set; }
        public int StaffId { get; set; }
        public DateTime CreateAt { get; set; } // Create At
        public DateTime EndDate { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Description { get; set; }
        public string? ImageWorkSpace { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Reason { get; set; }
        public int Status { get; set; }
        public Service_Schedule? Service_Schedule { get; set; }
        public Account? Account { get; set; }
        public AssignmentTask_Feedback? Feedback { get; set; }
        public IEnumerable<AssignmentTaskImage>? AssignmentTaskImages { get; set; }
    }
}
