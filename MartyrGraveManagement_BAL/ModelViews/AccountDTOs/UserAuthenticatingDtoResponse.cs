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
        public string AccountName { get; set; }
        public int? MartyrId { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
    }
}
