using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.GraveServiceDTOs
{
    public class UpdateServiceForGraveDtoRequest
    {
        [Required]
        public List<int>? ServiceId { get; set; } = new List<int>();
    }
}
