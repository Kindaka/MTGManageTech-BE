using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveGetAllForAdminDtoResponse
    {
        public string Code { get; set; } // MartyrCode
        public string Name { get; set; } // Name from MartyrGraveInformation
        public string martyrCode { get; set; }
        public string AreaDescription { get; set; }
        public string? GraveImage { get; set; }
        public string Location { get; set; } // AreaNumber + RowNumber + MartyrNumber
        public string RelativeName { get; set; } // FullName from Account
        public string RelativePhone { get; set; } // PhoneNumber from Account
        public int Status { get; set; } // Status from MartyrGrave
    }
}
