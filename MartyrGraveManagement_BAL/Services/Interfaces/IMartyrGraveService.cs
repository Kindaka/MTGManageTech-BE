using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IMartyrGraveService
    {
        Task<(List<MartyrGraveGetAllDtoResponse> matyrGraveList, int totalPage)> GetAllMartyrGravesAsync(int page, int pageSize);
        Task<MartyrGraveDtoResponse> GetMartyrGraveByIdAsync(int id);
        Task<List<MartyrGraveDtoResponse>> GetMartyrGraveByCustomerId(int customerId);
        //Task<MartyrGraveDtoResponse> CreateMartyrGraveAsync(MartyrGraveDtoRequest martyrGraveDto);
        Task<(bool status, string result, string? phone, string? password)> CreateMartyrGraveAsyncV2(MartyrGraveDtoRequest martyrGraveDto);
        Task<(bool status, string message)> ImportMartyrGraves(string excelFilePath, string outputFilePath);
        //Task<MartyrGraveDtoResponse> UpdateMartyrGraveAsync(int id, MartyrGraveDtoRequest martyrGraveDto);
        Task<bool> UpdateStatusMartyrGraveAsync(int id, int status);
        Task<(List<MartyrGraveGetAllForAdminDtoResponse> response, int totalPage)> GetAllMartyrGravesForManagerAsync(int page, int pageSize, int managerId);

        Task<(bool status, string result)> UpdateMartyrGraveAsyncV2(int id, MartyrGraveUpdateDtoRequest martyrGraveDto);
        Task<List<MartyrGraveSearchDtoResponse>> SearchMartyrGravesAsync(MartyrGraveSearchDtoRequest searchCriteria, int page, int pageSize);

        Task<(List<MartyrGraveByAreaDtoResponse> martyrGraves, int totalPage)> GetMartyrGraveByAreaIdAsync(int areaId, int pageIndex, int pageSize);

        Task<(List<MaintenanceHistoryDtoResponse> maintenanceHistory, int totalPage)> GetMaintenanceHistoryInMartyrGrave(int customerId, int martyrGraveId, int taskType, int pageIndex, int pageSize);
    }
}
