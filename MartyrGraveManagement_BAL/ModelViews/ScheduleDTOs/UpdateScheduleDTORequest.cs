using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs
{
    public class UpdateScheduleDTORequest
    {
        public DateTime Date { get; set; }
        public int SlotId { get; set; }
        public string Description { get; set; }
    }

}
