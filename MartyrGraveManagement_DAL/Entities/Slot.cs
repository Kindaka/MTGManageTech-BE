using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Slot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SlotId { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string SlotName { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Description { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool Status { get; set; }

        public IEnumerable<Schedule_Staff>? Schedules { get; set; }
    }
}
