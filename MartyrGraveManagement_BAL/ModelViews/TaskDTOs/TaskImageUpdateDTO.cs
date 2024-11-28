using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.TaskDTOs
{
    public class TaskImageUpdateDTO
    {
        [Required]
        public string ImageWorkSpace {  get; set; }
        [Required]
        public List<string> UrlImages { get; set; } = new List<string>();
    }

}
