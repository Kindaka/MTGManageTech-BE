using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AccountDTOs
{
    public class UserRoleDtoResponse
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string? CustomerCode { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

}
