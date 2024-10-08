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
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateOfSacrifice { get; set; }
        public string MartyrCode { get; set; }
    }
}
