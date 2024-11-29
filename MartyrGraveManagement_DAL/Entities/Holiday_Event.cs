using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Holiday_Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        public int AccountId { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string EventName { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }
        public DateOnly EventDate { get; set; }
        public bool Status { get; set; }

        public Account? Account { get; set; }
        public IEnumerable<Event_Image>? EventImages { get; set; }
    }
}
