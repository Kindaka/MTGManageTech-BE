using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveDtoResponse
    {
        public int MartyrId { get; set; }
        public int AreaId { get; set; }
        public string MartyrCode { get; set; }
        public int RowNumber { get; set; }
        public int MartyrNumber { get; set; }
        public int AreaNumber { get; set; }
        public bool Status { get; set; }

        // Thêm thuộc tính AreaName
        public string AreaName { get; set; }
    }
}
