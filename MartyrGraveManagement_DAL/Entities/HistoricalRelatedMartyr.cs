using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class HistoricalRelatedMartyr
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RelatedId { get; set; }
        public int BlogId { get; set; }
        public int InformationId { get; set; }
        public bool Status { get; set; }

        public MartyrGraveInformation? MartyrGraveInformation { get; set; }
        public Blog? Blog { get; set; }
    }
}
