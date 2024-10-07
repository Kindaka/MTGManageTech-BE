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
        [Required(ErrorMessage = "AreaId is required.")]
        public int AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "RowNumber must be a positive number.")]
        public int RowNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MartyrNumber must be a positive number.")]
        public int MartyrNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AreaNumber must be a positive number.")]
        public int AreaNumber { get; set; }

        [StringLength(250, ErrorMessage = "Address must be between 0 and 250 characters.")]
        public string Address { get; set; }

        public DateTime Dob { get; set; }

        public List<MartyrGraveInformationDtoRequest> Informations { get; set; } = new List<MartyrGraveInformationDtoRequest>();

        public List<GraveImageDtoRequest> Image { get; set; } = new List<GraveImageDtoRequest>();
    }
}
