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


        public async Task<bool> UpdateProfile(int accountId, UpdateProfileDtoRequest updateProfileDto)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy tài khoản.");
                }

                if (account.RoleId != 4)
                {
                    throw new UnauthorizedAccessException("Chỉ có tài khoản khách hàng mới được phép cập nhật thông tin.");
                }

                // Cập nhật các thông tin của khách hàng
                if (!string.IsNullOrEmpty(updateProfileDto.FullName))
                {
                    account.FullName = updateProfileDto.FullName;
                }

                if (updateProfileDto.DateOfBirth.HasValue)
                {
                    account.DateOfBirth = updateProfileDto.DateOfBirth.Value;
                }

                if (!string.IsNullOrEmpty(updateProfileDto.Address))
                {
                    account.Address = updateProfileDto.Address;
                }

                if (!string.IsNullOrEmpty(updateProfileDto.AvatarPath))
                {
                    account.AvatarPath = updateProfileDto.AvatarPath;
                }

                if (!string.IsNullOrEmpty(updateProfileDto.EmailAddress))
                {
                    account.EmailAddress = updateProfileDto.EmailAddress;
                }

                // Lưu thông tin đã cập nhật vào cơ sở dữ liệu
                await _unitOfWork.AccountRepository.UpdateAsync(account);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cập nhật thông tin thất bại: {ex.Message}");
            }
        }



    }
}
