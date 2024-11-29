using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.HolidayEventDTOs
{
    public class HolidayEventRequestDto
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "EventName is required.")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "EventDate is required.")]
        public EventDateDto EventDate { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>();

    }
}
