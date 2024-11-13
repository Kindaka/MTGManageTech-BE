using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.OtpDTOs
{
    public class OtpRequest
    {
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value?.Trim();
        }

        private string _phoneNumber;
    }
}
