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
        public string? Phone { get; set; }
        public string? Address { get; set; }
        [EmailAddress]
        public string? EmailAddress { get; set; }
        public DateTime Dob { get; set; }
    }
}
