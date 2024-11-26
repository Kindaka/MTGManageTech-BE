using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Schedule_Assignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentId { get; set; }
        public int ServiceScheduleId { get; set; }
        public int StaffId { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Reason { get; set; }
        public int Status { get; set; }

        public Service_Schedule? Service_Schedule { get; set; }
        public Account? Account { get; set; }

        public IEnumerable<AssignmentTask>? AssignmentTasks { get; set; }
    }
}
