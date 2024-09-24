using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class WeeklyReportGrave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WeeklyReportId { get; set; }
        public int MartyrId { get; set; }
        public int AccountId { get; set; }
        public int QualityOfTotalGravePoint { get; set; }
        public int QualityOfFlowerPoint { get; set; }
        public int DisciplinePoint { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

        public MartyrGrave? MartyrGrave { get; set; }

    }

}
