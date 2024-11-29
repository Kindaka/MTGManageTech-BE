using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class TaskImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        public int TaskId { get; set; }
        public string? ImageWorkSpace { get; set; }
        public DateTime CreateAt { get; set; }

        public StaffTask? StaffTask { get; set; }
    }
}
