using MartyrGraveManagement_BAL.ModelViews.RequestCustomerDTOs;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IRequestCustomerService
    {
        Task<(bool status, string response)> CreateRequestsAsync(RequestCustomerDtoRequest request);
        Task<(bool status, string response)> AcceptRequestForManagerAsync(RequestCustomerDtoManagerResponse dtoManagerResponse);
        Task<(bool status, string response)> AcceptServiceRequestForCustomerAsync(int requestId, int customerId);

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
