using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Service_Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceScheduleId { get; set; }
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public int MartyrId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public DateOnly ScheduleDate { get; set; }
        public int DayOfMonth { get; set; } = 0;
        public int DayOfWeek { get; set; } = 0;
        public string? Note { get; set; }
        public bool Status { get; set; }

        public Account? Account { get; set; }
        public Service? Service { get; set; }
        public MartyrGrave? MartyrGrave { get; set; }

        public IEnumerable<Schedule_Assignment>? ScheduleAssignments { get; set; }
    }
}
