using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    [Table("Schedule")]
    public class Schedule
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScheduleId { get; set; }
        public int AccountId { get; set; }
        public int SlotId { get; set; }
        public int? TaskId { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string? Description { get; set; }
        public int Status { get; set; }

        public Slot? Slot { get; set; }  
        public IEnumerable<ScheduleDetail>? ScheduleTasks { get; set; }  
        public IEnumerable<Attendance>? Attendances { get; set; }
    }
}
