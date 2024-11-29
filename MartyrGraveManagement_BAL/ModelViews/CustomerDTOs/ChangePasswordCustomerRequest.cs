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
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value?.Trim();
        }

        private string _phoneNumber;
        [Required]
        public string OldPassword
        {
            get => _oldPassword;
            set => _oldPassword = value?.Trim();
        }

        private string _oldPassword;
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
