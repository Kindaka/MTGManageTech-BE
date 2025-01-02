using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.StaffDTOs
{
    public class StaffDtoResponse
    {
        public int AccountId { get; set; }
        public string? StaffFullName { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
