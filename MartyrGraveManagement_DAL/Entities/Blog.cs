using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get; set; }
        public int AccountId { get; set; }
        public int HistoryId { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string BlogName { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string BlogContent { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }


        public IEnumerable<HistoricalImage>? HistoricalImages { get; set; }
        public IEnumerable<Comment>? Comments { get; set; }
        public IEnumerable<HistoricalRelatedMartyr>? HistoricalRelatedMartyrs { get; set; }
        public Account? Account { get; set; }
        public HistoricalEvent? HistoricalEvent { get; set; }
    }
}
