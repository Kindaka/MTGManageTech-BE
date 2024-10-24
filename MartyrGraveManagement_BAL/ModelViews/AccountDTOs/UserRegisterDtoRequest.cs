﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AccountDTOs
{
    public class UserRegisterDtoRequest
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be numeric and exactly 10 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
        [StringLength(50, ErrorMessage = "FullName must be between 0 and 50 characters.")]
        public string FullName { get; set; } = null!;
        [EmailAddress]
        public string EmailAddress { get; set; } = null!;
        [StringLength(30, ErrorMessage = "Address must be between 0 and 64 characters.")]
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public int? AreaId { get; set; }
    }
}
