using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        private async Task<string> HashPassword(string password)
        {
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        stringBuilder.Append(hashBytes[i].ToString("x2"));
                    }

                    return await Task.FromResult(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error hashing password: {ex.Message}");
            }
        }
        public async Task<(bool status, string responseContent)> ChangePasswordCustomer(ChangePasswordCustomerRequest request)
        {
            try
            {
                string hashedPassword = await HashPassword(request.OldPassword);
                var existedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.PhoneNumber == request.PhoneNumber && a.HashedPassword == hashedPassword)).FirstOrDefault();
                if (existedAccount != null)
                {

                    existedAccount.HashedPassword = await HashPassword(request.Password);
                    await _unitOfWork.AccountRepository.UpdateAsync(existedAccount);
                    await _unitOfWork.SaveAsync();
                    return (true, "Change password successfully");
                }
                else
                {
                    return (false, "Email or Password not true, check again");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
