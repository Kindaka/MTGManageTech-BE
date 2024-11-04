using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AttendanceDTOs
{
    public class CheckAttendancesDtoRequest
    {
        public int attendanceId { get; set; }
        public int statusAttendance {  get; set; }
    }
}
