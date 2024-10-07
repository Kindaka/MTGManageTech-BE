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
        Task<List<MartyrGraveDtoResponse>> GetAllMartyrGravesAsync();
        Task<MartyrGraveDtoResponse> GetMartyrGraveByIdAsync(int id);
        Task<MartyrGraveDtoResponse> CreateMartyrGraveAsync(MartyrGraveDtoRequest martyrGraveDto);
        Task<(bool status, string result, string? accountName, string? password)> CreateMartyrGraveAsyncV2(MartyrGraveDtoRequest martyrGraveDto);
        Task<(bool status, string result, string? accountName, string? password)> CreateRelativeGraveAsync(int martyrGraveId,CustomerDtoRequest customer);
        Task<MartyrGraveDtoResponse> UpdateMartyrGraveAsync(int id, MartyrGraveDtoRequest martyrGraveDto);
        Task<bool> UpdateStatusMartyrGraveAsync(int id);
        Task<(List<MartyrGraveGetAllDtoResponse> response, int totalPage)> GetAllMartyrGravesForManagerAsync(int page, int pageSize);

        Task<(bool status, string result)> UpdateMartyrGraveAsyncV2(int id, MartyrGraveDtoRequest martyrGraveDto);

    }
}
