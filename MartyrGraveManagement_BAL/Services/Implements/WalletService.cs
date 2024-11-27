using MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System.Linq.Expressions;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Lấy lịch sử giao dịch của khách hàng với phân trang
        public async Task<(List<TransactionBalanceHistoryDTO> transactions, int totalPages)> GetTransactionHistory(
            int customerId,
            int pageIndex = 1,
            int pageSize = 10,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                Expression<Func<TransactionBalanceHistory, bool>> filter = t => t.CustomerId == customerId;

                if (startDate.HasValue && endDate.HasValue)
                {
                    filter = t => t.CustomerId == customerId
                        && t.TransactionDate.Date >= startDate.Value.Date
                        && t.TransactionDate.Date <= endDate.Value.Date;
                }

                var totalTransactions = await _unitOfWork.TransactionBalanceHistoryRepository.CountAsync(filter);
                var totalPages = (int)Math.Ceiling(totalTransactions / (double)pageSize);

                var transactions = await _unitOfWork.TransactionBalanceHistoryRepository.GetAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(t => t.TransactionDate),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                var transactionDtos = transactions.Select(t => new TransactionBalanceHistoryDTO
                {
                    TransactionId = t.TransactionId,
                    CustomerId = t.CustomerId,
                    TransactionType = t.TransactionType,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    BalanceAfterTransaction = t.BalanceAfterTransaction
                }).ToList();

                return (transactionDtos, totalPages);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting transaction history: {ex.Message}");
            }
        }

        // Lấy thông tin ví của khách hàng
        public async Task<CustomerWalletDTO> GetCustomerWallet(int customerId)
        {
            try
            {
                var wallet = (await _unitOfWork.CustomerWalletRepository
                    .GetAsync(w => w.CustomerId == customerId))
                    .FirstOrDefault();

                if (wallet == null)
                {
                    return new CustomerWalletDTO
                    {
                        CustomerId = customerId,
                        CustomerBalance = 0,
                        UpdateAt = DateTime.Now
                    };
                }

                return new CustomerWalletDTO
                {
                    WalletId = wallet.WalletId,
                    CustomerId = wallet.CustomerId,
                    CustomerBalance = wallet.CustomerBalance,
                    UpdateAt = wallet.UpdateAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting customer wallet: {ex.Message}");
            }
        }
    }
}
