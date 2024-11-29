using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveSearchDtoResponse
    {
        public int MartyrId { get; set; }
        public string Name { get; set; }
        public string? NickName { get; set; }
        public string? HomeTown { get; set; }
        public string? DateOfBirth { get; set; }
        public string? DateOfSacrifice { get; set; }
        public string MartyrCode { get; set; }
        public string? GraveLocation { get; set; }

        public List<GraveImageDtoResponse> ImageUrls { get; set; }

    }
}
