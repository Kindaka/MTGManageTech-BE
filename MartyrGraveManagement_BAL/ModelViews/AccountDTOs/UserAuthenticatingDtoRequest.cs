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
        [StringLength(64, ErrorMessage = "AccountName must be between 0 and 64 characters.")]
        public string? AccountName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
