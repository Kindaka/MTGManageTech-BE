using MartyrGraveManagement_BAL.ModelViews.AccountDTOs;
using MartyrGraveManagement_BAL.ModelViews.AreaDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<(List<AccountDtoResponse> staffList, int totalPage)> GetStaffList(int page, int pageSize);
        Task<(List<AccountDtoResponse> managerList, int totalPage)> GetManagerList(int page, int pageSize);
<<<<<<< Updated upstream
        Task<bool> ChangeStatusUser(int accountId);
=======
        Task<bool> UpdateProfileForStaffOrManager(int accountId, UpdateProfileStaffOrManagerDtoRequest updateProfileDto);
        Task<bool> ChangeStatusUser(int accountId, int userAccountId);
        Task<AccountDtoResponse> GetAccountProfile(int accountId);
        Task<Dictionary<int, int>> GetTotalAccountsByRolesAsync(IEnumerable<int> roleIds);
>>>>>>> Stashed changes
    }
}
