using MartyrGraveManagement_BAL.ModelViews.CustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IMartyrGraveService
    {
        Task<(List<MartyrGraveGetAllDtoResponse> matyrGraveList, int totalPage)> GetAllMartyrGravesAsync(int page, int pageSize);
        Task<MartyrGraveDtoResponse> GetMartyrGraveByIdAsync(int id);
        Task<List<MartyrGraveDtoResponse>> GetMartyrGraveByCustomerCode(string customerCode);
        Task<MartyrGraveDtoResponse> CreateMartyrGraveAsync(MartyrGraveDtoRequest martyrGraveDto);
        Task<(bool status, string result, string? phone, string? password)> CreateMartyrGraveAsyncV2(MartyrGraveDtoRequest martyrGraveDto);
        Task<MartyrGraveDtoResponse> UpdateMartyrGraveAsync(int id, MartyrGraveDtoRequest martyrGraveDto);
        Task<bool> UpdateStatusMartyrGraveAsync(int id, int status);
        Task<(List<MartyrGraveGetAllForAdminDtoResponse> response, int totalPage)> GetAllMartyrGravesForManagerAsync(int page, int pageSize);

        Task<(bool status, string result)> UpdateMartyrGraveAsyncV2(int id, MartyrGraveUpdateDtoRequest martyrGraveDto);
        Task<List<MartyrGraveSearchDtoResponse>> SearchMartyrGravesAsync(MartyrGraveSearchDtoRequest searchCriteria);

    }
}
