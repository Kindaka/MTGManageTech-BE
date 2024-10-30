using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AccountDTOs
{
    public class AccountDtoResponse
    {
        public int AccountId { get; set; }
        public string? FullName { get; set; }
        public DateTime CreateAt { get; set; }
        public bool Status { get; set; }
        public int? AreaId { get; set; }
        public string? EmailAddress { get; set; }
        public string phoneNumber { get; set; }
        public string? Address { get; set; }
        public string? AvatarPath { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int RoleId { get; set; }
    }
}
