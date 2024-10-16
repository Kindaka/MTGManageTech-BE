using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserAuthenticatingDtoResponse?> AuthenticateUser(UserAuthenticatingDtoRequest loginInfo)
        {
            try
            {
                UserAuthenticatingDtoResponse response = new UserAuthenticatingDtoResponse();
                string hashedPassword = await HashPassword(loginInfo.Password);

                var account = (await _unitOfWork.AccountRepository
                    .FindAsync(a => a.AccountName == loginInfo.AccountName && a.HashedPassword == hashedPassword))
                    .FirstOrDefault();

                if (account != null)
                {
                    if (!string.IsNullOrEmpty(account.CustomerCode))
                    {
                        response.customerCode = account.CustomerCode;
                    }
                    else
                    {
                        response.customerCode = "";
                    }

                    response.AccountId = account.AccountId;
                    response.AccountName = account.AccountName;

                    response.RoleId = account.RoleId;
                    response.Status = account.Status;

                    if (account.RoleId == 3)
                    {
                        response.AreaId = account.AreaId;
                    }

                    return response;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error authenticating user: {ex.Message}");
            }
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

        public async Task<bool> CreateAccount(UserRegisterDtoRequest newAccount)
        {
            try
            {
                // Kiểm tra xem email đã tồn tại chưa
                var existingAccount = await _unitOfWork.AccountRepository
                    .FindAsync(a => a.AccountName == newAccount.AccountName);
                if (existingAccount.Any())
                {
                    throw new Exception("Email đã tồn tại.");
                }

                // Hash mật khẩu
                newAccount.Password = await HashPassword(newAccount.Password);

                var existingRole = await _unitOfWork.RoleRepository.GetByIDAsync(newAccount.RoleId);
                if (existingRole == null)
                {
                    return false;
                }

                if (existingRole.RoleId == 3)
                {
                    if (newAccount.AreaId == null)
                    {
                        return false;
                    }
                }

                if (newAccount.RoleId == 3)
                {
                    var account = _mapper.Map<Account>(newAccount);
                    account.Status = true;
                    account.CreateAt = DateTime.Now;

                    // Lưu tài khoản vào cơ sở dữ liệu
                    await _unitOfWork.AccountRepository.AddAsync(account);
                    await _unitOfWork.SaveAsync();

                    return true; // Lưu thành công
                }
                else
                {
                    var account = _mapper.Map<Account>(newAccount);
                    account.Status = true;
                    account.AreaId = null;
                    account.CreateAt = DateTime.Now;

                    // Lưu tài khoản vào cơ sở dữ liệu
                    await _unitOfWork.AccountRepository.AddAsync(account);
                    await _unitOfWork.SaveAsync();

                    return true; // Lưu thành công
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo tài khoản: {ex.Message}");
            }
        }


        public async Task<string> GenerateAccessToken(UserAuthenticatingDtoResponse account)
        {
            try
            {
                if (account == null || account.RoleId <= 0)
                {
                    throw new Exception("Thông tin tài khoản không hợp lệ.");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim("accountId", account.AccountId.ToString()),
                    new Claim("accountName", account.AccountName),
                    new Claim("customerCode", account.customerCode),
                    new Claim(ClaimTypes.Role, account.RoleId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, account.AccountId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                if (account.RoleId == 3 && account.AreaId.HasValue)
                {
                    claims.Add(new Claim("areaId", account.AreaId.Value.ToString()));
                }

                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo Access Token: {ex.Message}");
            }
        }

        public async Task<bool> GetAccountByAccountName(string accountName)
        {
            try
            {
                var account = (await _unitOfWork.AccountRepository.GetAsync(c => c.AccountName == accountName)).FirstOrDefault();
                if (account == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm tài khoản qua name: {ex.Message}");
            }
        }
    }
}
