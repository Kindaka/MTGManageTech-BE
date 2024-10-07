using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AuthorizeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorizeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool isMatchedCustomer, bool isAuthorizedAccount)> CheckAuthorizeByAccountId(int userAccountId, int accountId)
        {
            try
            {
                bool isAuthorizedAccount = false;
                bool isMatchedCustomer = false;
                var account = (await _unitOfWork.AccountRepository.GetByIDAsync(userAccountId));
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
                    if (accountJwt.RoleId == 1 || accountJwt.RoleId == 2)
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
    }
}
