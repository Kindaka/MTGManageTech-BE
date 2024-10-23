using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CustomerDTOs
{
    public class ChangePasswordCustomerRequest
    {
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string OldPassword { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
    }
}
