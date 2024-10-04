using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IOdersService
    {
        Task<List<OrdersGetAllDTOResponse>> GetAllOrders();
        Task<OrdersGetAllDTOResponse> GetOrderById(int orderId);
        Task<OrdersDTOResponse> CreateOrderFromCartAsync(int accountId);
        Task<List<OrdersGetAllDTOResponse>> GetOrderByAccountId(int accountId);
        Task<bool> UpdateOrderStatus(int orderId, int newStatus);
        Task<bool> DeleteAsync(int id);
    }
}
