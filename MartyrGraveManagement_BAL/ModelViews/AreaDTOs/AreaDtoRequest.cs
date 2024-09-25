using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AreaDTos
{
    public class AreaDtoRequest
    {
        [Required]
        public string AreaName { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
