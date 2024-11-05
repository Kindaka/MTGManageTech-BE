using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.HolidayEventDTOs
{
    public class HolidayEventResponseDto
    {
        public int EventId { get; set; }
        public int AccountId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public DateOnly EventDate { get; set; }
        public bool Status { get; set; }

        public string AccountFullName { get; set; }

        public List<string> EventImages { get; set; } = new List<string>();
    }
}
