using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MartyrGraveManagement_BAL.ModelViews.AccountDTOs
{
    public class UserRegisterDtoRequest
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
        [StringLength(50, ErrorMessage = "FullName must be between 0 and 50 characters.")]
        public string FullName { get; set; } = null!;
        [EmailAddress]
        public string EmailAddress { get; set; } = null!;
        [AllowNull]
        [StringLength(64, ErrorMessage = "Address must be between 0 and 64 characters.")]
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        [Required]
        public int AreaId { get; set; }
    }
}
