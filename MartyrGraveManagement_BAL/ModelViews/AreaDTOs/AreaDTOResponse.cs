using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AreaDTOs
{
    public class AreaDTOResponse
    {

        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

    }
}
