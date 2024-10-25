using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.CustomerDTOs
{
    public class CustomerDtoRequest
    {

        public string? UserName { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be numeric and exactly 10 digits.")]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [EmailAddress]
        public string? EmailAddress { get; set; }
        public DateTime Dob { get; set; }
    }
}
