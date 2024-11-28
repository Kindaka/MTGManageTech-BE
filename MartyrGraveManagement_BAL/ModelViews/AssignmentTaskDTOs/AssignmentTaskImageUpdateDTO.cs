using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDTOs
{
    public class AssignmentTaskImageUpdateDTO
    {
        public string ImageWorkSpace { get; set; }
        public List<string> UrlImages { get; set; } = new List<string>();
    }
}
