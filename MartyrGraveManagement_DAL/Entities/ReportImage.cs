using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class ReportImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        public int ReportId { get; set; }
        public string? UrlPath { get; set; }
        public DateTime CreateAt { get; set; }
        public ReportGrave? ReportGrave { get; set; }
    }
}
