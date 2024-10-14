using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveGetAllDtoResponse
    {
        public int MartyrId { get; set; }
        public int AreaId { get; set; }
        public string MartyrCode { get; set; }
        public List<string>? Name { get; set; } = new List<string>();
        public string? image { get; set; } = null;
    }
}
