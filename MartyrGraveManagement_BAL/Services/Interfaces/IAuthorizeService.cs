namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAuthorizeService
    {
        Task<(bool isMatchedAccount, bool isAuthorizedAccount)> CheckAuthorizeByAccountId(int userAccountId, int accountId);
        Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByCustomerId(int customerId, int accountId);
        Task<(bool isMatchedStaffOrManager, bool isAuthorized)> CheckAuthorizeStaffOrManager(int userAccountId, int accountId);
        Task<(bool isMatchedAccountStaff, bool isAuthorizedAccount)> CheckAuthorizeStaffByAccountId(int userAccountId, int accountId);
        Task<(bool isMatchedAccountManager, bool isAuthorizedAccount)> CheckAuthorizeManagerByAccountId(int userAccountId, int accountId);
        Task<(bool isMatchedAccount, bool isAuthorizedAccount)> CheckAuthorizeManagerOrAdmin(int userAccountId, int accountId);
        Task<bool> CheckAuthorizeByCartId(int cartId, int customerId);
        //Task<bool> CheckAuthorizeByFeedbackId(int feedbackId, int customerId);
        Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByOrderId(int orderId, int accountId);

        Task<bool> CheckAuthorizeStaffByAreaId(int taskId, int accountId, int areaId);
        Task<bool> CheckAuthorizeStaffByAreaId(int accountId, int areaId);
    }
}
