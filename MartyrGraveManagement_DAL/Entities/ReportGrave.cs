using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class ReportGrave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportId { get; set; }
        public int RequestId { get; set; }
        public string VideoFile { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool Status { get; set; }

        public RequestCustomer? RequestCustomer { get; set; }
        public IEnumerable<ReportImage>? ReportImages { get; set; }
    }

}
