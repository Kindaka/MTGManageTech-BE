using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<(bool status, string responseContent)> ChangePasswordCustomer(ChangePasswordCustomerRequest request);
        Task<bool> UpdateProfile(int accountId, UpdateProfileDtoRequest updateProfileDto);

    }
}
