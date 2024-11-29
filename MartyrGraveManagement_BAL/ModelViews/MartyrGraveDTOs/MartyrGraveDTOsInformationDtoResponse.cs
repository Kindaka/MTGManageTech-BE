using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveDTOsInformationDtoResponse
    {
        public int InformationId { get; set; } // ID thông tin liệt sĩ
        public string Name { get; set; } // Tên liệt sĩ
        public string NickName { get; set; } // Tên thân mật
        public string Position { get; set; } // Chức vụ
        public string Medal { get; set; } // Huy chương
        public string HomeTown { get; set; } // Quê quán
        public string? DateOfBirth { get; set; } // Ngày sinh
        public string? DateOfSacrifice { get; set; } // Ngày hy sinh
    }

}
