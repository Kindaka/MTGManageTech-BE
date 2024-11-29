using MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IWalletService
    {
        Task<(List<TransactionBalanceHistoryDTO> transactions, int totalPages)> GetTransactionHistory(
            int customerId, int pageIndex = 1, int pageSize = 10, 
            DateTime? startDate = null, DateTime? endDate = null);
        
        Task<CustomerWalletDTO> GetCustomerWallet(int customerId);
    }
}
