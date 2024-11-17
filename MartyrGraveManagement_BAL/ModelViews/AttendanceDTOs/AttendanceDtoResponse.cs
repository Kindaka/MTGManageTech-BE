using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs
{
    public class AttendanceDtoResponse
    {
        public int AttendanceId { get; set; }
        public int AccountId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int SlotId { get; set; }
        public string staffName { get; set; }
        public DateOnly Date { get; set; } 
        public TimeOnly StartTime { get; set; } 
        public TimeOnly EndTime { get; set; }
        public string? ImagePath1 { get; set; }
        public string? ImagePath2 { get; set; }
        public string? ImagePath3 { get; set; }
        public int status { get; set; }
    }
}
