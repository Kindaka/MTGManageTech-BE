using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IAuthorizeService
    {
        Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByAccountId(int userAccountId, int accountId);
        Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByCustomerId(int customerId, int accountId);
        Task<(bool isMatchedAccountStaff, bool isAuthorizedAccount)> CheckAuthorizeStaffByAccountId(int userAccountId, int accountId);
        Task<bool> CheckAuthorizeByCartId(int cartId, int customerId);
        //Task<bool> CheckAuthorizeByFeedbackId(int feedbackId, int customerId);
        Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByOrderId(int orderId, int accountId);
    }
}
