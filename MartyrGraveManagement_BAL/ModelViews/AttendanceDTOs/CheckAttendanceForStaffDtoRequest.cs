using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs
{
    public class CheckAttendanceForStaffDtoRequest
    {
        [Required]
        public int AttendanceId { get; set; }
        [Required]
        public string ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; }
        public string? ImagePath3 { get; set; }
    }
}
