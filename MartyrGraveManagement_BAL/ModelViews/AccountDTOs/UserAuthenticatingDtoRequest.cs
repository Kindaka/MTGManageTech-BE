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
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
