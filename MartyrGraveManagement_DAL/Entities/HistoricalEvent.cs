using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class HistoricalEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string HistoryEventName { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Status { get; set; }


        public ICollection<HistoricalImage>? HistoricalImages { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public IEnumerable<HistoricalRelatedMartyr>? HistoricalRelatedMartyrs { get; set; }
    }
}
