using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs
{
    public class MartyrGraveInformationDtoResponse
    {
        public int InformationId { get; set; }
        public int MartyrId { get; set; }
        public string Name { get; set; }
        public string Medal { get; set; }
        public DateTime DateOfSacrifice { get; set; }
    }
}
