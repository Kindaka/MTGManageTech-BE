using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AccountDTOs
{
    public class UserAuthenticatingDtoResponse
    {
        public int AccountId { get; set; }
        public string PhoneNumber { get; set; }
        public string? customerCode { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
        public int? AreaId { get; set; }
    }
}
