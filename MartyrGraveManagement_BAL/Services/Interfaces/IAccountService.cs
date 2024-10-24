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
        Task<(List<AccountDtoResponse> staffList, int totalPage)> GetStaffList(int page, int pageSize, int? areaId = null);
        Task<(List<AccountDtoResponse> managerList, int totalPage)> GetManagerList(int page, int pageSize);
        Task<bool> UpdateProfileForStaffOrManager(int accountId, UpdateProfileStaffOrManagerDtoRequest updateProfileDto);
        Task<bool> ChangeStatusUser(int accountId);
    }
}
