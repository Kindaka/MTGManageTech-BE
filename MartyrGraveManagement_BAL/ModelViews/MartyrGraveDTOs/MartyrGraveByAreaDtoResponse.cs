using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveByAreaDtoResponse
    {
        public int MartyrId { get; set; } // ID của mộ liệt sĩ
        public string MartyrCode { get; set; } // Mã mộ liệt sĩ
        public int Status { get; set; } // Trạng thái mộ
        public string AreaName { get; set; } // Tên khu vực
        public string LocationDescription { get; set; } // Mô tả vị trí (AreaNumber-RowNumber-MartyrNumber)
        public List<GraveImageDtoResponse> Images { get; set; } = new List<GraveImageDtoResponse>(); // Danh sách ảnh
        public List<MartyrGraveDTOsInformationDtoResponse> MatyrGraveInformations { get; set; } = new List<MartyrGraveDTOsInformationDtoResponse>(); // Danh sách thông tin liệt sĩ
    }

}
