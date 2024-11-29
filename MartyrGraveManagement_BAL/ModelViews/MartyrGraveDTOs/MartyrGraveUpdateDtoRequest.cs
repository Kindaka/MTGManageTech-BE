using MartyrGraveManagement_BAL.ModelViews.MartyrGraveInformationDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs
{
    public class MartyrGraveUpdateDtoRequest
    {
        public List<MartyrGraveInformationDtoRequest> Informations { get; set; } = new List<MartyrGraveInformationDtoRequest>();

        public List<GraveImageDtoRequest> Image { get; set; } = new List<GraveImageDtoRequest>();
    }
}
