using MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs;
using MartyrGraveManagement_BAL.ModelViews.RequestMaterialDTOs;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IRequestCustomerService
    {
        Task<(bool status, string response)> CreateRequestsAsync(RequestCustomerDtoRequest request);
        Task<(bool status, string response)> AcceptRequestForManagerAsync(int requestId, int managerId, RequestMaterialDtoRequest? requestMaterial);

        Task<(IEnumerable<RequestCustomerDtoResponse> requestList, int totalPage)> GetRequestsByAccountIdAsync(
            int accountId,
            int pageIndex,
            int pageSize,
            DateTime Date);

        Task<(IEnumerable<RequestCustomerDtoResponse> requestList, int totalPage)> GetRequestsForManager(
            int managerId,
            int pageIndex,
            int pageSize,
            DateTime Date);

        Task<RequestCustomerDtoResponse> GetRequestByIdAsync(int taskId);
    }
}
