using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class AssignmentTaskImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        public int AssignmentTaskId { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreateAt { get; set; }

        public AssignmentTask? AssignmentTask { get; set; }
    }
}
