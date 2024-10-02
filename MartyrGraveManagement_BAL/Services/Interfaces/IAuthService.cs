using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserAuthenticatingDtoResponse?> AuthenticateUser(UserAuthenticatingDtoRequest loginInfo);

        Task<string> GenerateAccessToken(UserAuthenticatingDtoResponse account);

        Task<bool> CreateAccount(UserRegisterDtoRequest newAccount);

        Task<bool> GetAccountByAccountName(string accountName);

    }
}
