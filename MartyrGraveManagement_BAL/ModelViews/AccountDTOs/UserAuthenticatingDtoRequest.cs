using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.AccountDTOs
{
    public class UserAuthenticatingDtoRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(64, ErrorMessage = "Email must be between 0 and 64 characters.")]
        public string? EmailAddress { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
