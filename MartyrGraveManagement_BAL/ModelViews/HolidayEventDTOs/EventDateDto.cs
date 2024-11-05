using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.HolidayEventDTOs
{
    public class EventDateDto
    {
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; }

        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31.")]
        public int Day { get; set; }
    }
}
