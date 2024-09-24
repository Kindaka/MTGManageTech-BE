using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class WorkPerformance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkId { get; set; }
        public int AccountId { get; set; }
        public int QualityMaintenancePoint { get; set; }
        public int TimeCompletePoint { get; set; }
        public int InteractionPoint { get; set; }
        public string Description { get; set; }
        public DateTime UploadTime { get; set; }
        public bool Status { get; set; }

        public Account? Account { get; set; }
    }

}
