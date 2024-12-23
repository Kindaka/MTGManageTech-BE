using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.RequestTypeDTOs
{
    public class RequestTypeResponse
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string? TypeDescription { get; set; }
        public bool Status { get; set; }
    }
}
