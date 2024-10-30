using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorizeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool isMatchedAccount, bool isAuthorizedAccount)> CheckAuthorizeByAccountId(int userAccountId, int accountId)
        {
            try
            {
                bool isAuthorizedAccount = false;
                bool isMatchedAccount = false;
                var account = (await _unitOfWork.AccountRepository.GetByIDAsync(userAccountId));
                if (account != null)
                {
                    if (account.AccountId == accountId)
                    {
                        isMatchedAccount = true;
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.RoleId == 1 || accountJwt.RoleId == 2 || accountJwt.RoleId == 3 || accountJwt.RoleId == 4)
                    {
                        isAuthorizedAccount = true;
                    }
                }
                return (isMatchedAccount, isAuthorizedAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isMatchedStaffOrManager, bool isAuthorized)> CheckAuthorizeStaffOrManager(int userAccountId, int accountId)
        {
            try
            {
                bool isAuthorized = false;
                bool isMatchedStaffOrManager = false;

                // Lấy thông tin tài khoản từ userAccountId (người dùng hiện tại)
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(userAccountId);
                if (account != null && account.AccountId == accountId)
                {
                    isMatchedStaffOrManager = true;
                }

                // Lấy thông tin tài khoản để kiểm tra quyền
                var accountToCheck = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountToCheck != null)
                {
                    // Kiểm tra nếu tài khoản là quản lý (RoleId == 2) hoặc nhân viên (RoleId == 3)
                    if (accountToCheck.RoleId == 2 || accountToCheck.RoleId == 3)
                    {
                        isAuthorized = true;
                    }
                }

                return (isMatchedStaffOrManager, isAuthorized);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in authorization check: {ex.Message}");
            }
        }


        public async Task<bool> CheckAuthorizeByCartId(int cartId, int customerId)
        {
            try
            {
                var cart = (await _unitOfWork.CartItemRepository.GetByIDAsync(cartId));
                if (cart != null)
                {
                    if (cart.AccountId == customerId)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByCustomerId(int customerId, int accountId)
        {
            try
            {
                bool isAuthorizedAccount = false;
                bool isMatchedCustomer = false;
                var account = (await _unitOfWork.AccountRepository.GetByIDAsync(customerId));
                if (account != null)
                {
                    if (account.AccountId == accountId)
                    {
                        isMatchedCustomer = true;
                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.RoleId == 4)
                    {
                        isAuthorizedAccount = true;
                    }
                }
                return (isMatchedCustomer, isAuthorizedAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public async Task<bool> CheckAuthorizeByFeedbackId(int feedbackId, int customerId)
        //{
        //    try
        //    {
        //        var feedback = (await _unitOfWork.FeedbackRepository.GetByIDAsync(feedbackId));
        //        if (feedback != null)
        //        {
        //            if (feedback.CustomerId == customerId)
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public async Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByOrderId(int orderId, int accountId)
        {
            try
            {
                bool isAuthorizedAccount = false;
                bool isMatchedCustomer = false;
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.RoleId == 1 || accountJwt.RoleId == 2)
                    {
                        isAuthorizedAccount = true;
                    }
                }
                var order = (await _unitOfWork.OrderRepository.GetByIDAsync(orderId));
                if (order != null)
                {
                    var customer = await _unitOfWork.AccountRepository.GetByIDAsync(order.AccountId);
                    if (customer.AccountId == accountId)
                    {
                        isMatchedCustomer = true;
                    }
                }
                return (isMatchedCustomer, isAuthorizedAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isMatchedAccountStaff, bool isAuthorizedAccount)> CheckAuthorizeStaffByAccountId(int userAccountId, int accountId)
        {
            try
            {
                bool isAuthorizedAccount = false;
                bool isMatchedAccountStaff = false;
                var account = (await _unitOfWork.AccountRepository.GetByIDAsync(userAccountId));
                if (account != null)
                {
                    if (account.AccountId == accountId)
                    {
                        if (account.RoleId == 2 || account.RoleId == 4)
                        {
                            isMatchedAccountStaff = true;
                        }

                    }
                }
                var accountJwt = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (accountJwt != null)
                {
                    if (accountJwt.RoleId == 4 || accountJwt.RoleId == 2)
                    {
                        isAuthorizedAccount = true;
                    }
                }
                return (isMatchedAccountStaff, isAuthorizedAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CheckAuthorizeStaffByAreaId(int taskId, int accountId, int areaId)
        {
            try
            {
                // Lấy thông tin Task dựa trên taskId
                var task = await _unitOfWork.TaskRepository.GetByIDAsync(taskId);
                if (task == null)
                {
                    throw new KeyNotFoundException("TaskId does not exist.");
                }

                // Lấy thông tin Account (nhân viên) dựa trên accountId
                var staffAccount = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (staffAccount == null || staffAccount.RoleId != 3)
                {
                    throw new UnauthorizedAccessException("The account is not a valid staff account.");
                }

                // Lấy thông tin OrderDetail liên quan đến task
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIDAsync(task.DetailId);
                if (orderDetail == null)
                {
                    throw new KeyNotFoundException("OrderDetail does not exist for this task.");
                }

                // Lấy thông tin MartyrGrave từ OrderDetail
                var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(orderDetail.MartyrId);
                if (martyrGrave == null)
                {
                    throw new KeyNotFoundException("MartyrGrave does not exist.");
                }

                // Kiểm tra xem nhân viên có thuộc AreaId của MartyrGrave không và task này có thuộc về nhân viên này không
                if (martyrGrave.AreaId != areaId || task.AccountId != accountId)
                {
                    return false; // Nhân viên không thuộc khu vực này hoặc không phải là nhân viên phụ trách task này
                }

                // Nhân viên thuộc khu vực và có quyền làm việc với task này
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Authorization check failed: {ex.Message}");
            }
        }




    }
}
