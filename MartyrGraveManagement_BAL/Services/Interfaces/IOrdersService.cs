using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IOrdersService
    {
        Task<List<OrdersGetAllDTOResponse>> GetAllOrders();
        Task<OrdersGetAllDTOResponse> GetOrderById(int orderId);
        Task<(bool status, string? paymentUrl, string responseContent)> CreateOrderFromCartAsync(int accountId, OrdersDTORequest orderBody);
        Task<List<OrdersGetAllDTOResponse>> GetOrderByAccountId(int accountId);
        Task<List<OrdersGetAllDTOResponse>> GetOrderByAreaId(int areaId);
        Task<bool> UpdateOrderStatus(int orderId, int newStatus);
        Task<bool> DeleteAsync(int id);
    }
}
