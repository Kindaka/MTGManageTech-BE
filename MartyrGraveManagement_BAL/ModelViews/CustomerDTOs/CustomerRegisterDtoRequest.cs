using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CustomerDTOs
{
    public class CustomerRegisterDtoRequest
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be numeric and exactly 10 digits.")]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value?.Trim();
        }

        private string _phoneNumber;
        [Required]
        public string Password
        {
            get => _password;
            set => _password = value?.Trim();
        }

        private string _password;
        [Required]
        public string ConfirmPassword 
        {
            get => _confirmPassword;
            set => _confirmPassword = value?.Trim();
        }

        private string _confirmPassword;
    }
}
