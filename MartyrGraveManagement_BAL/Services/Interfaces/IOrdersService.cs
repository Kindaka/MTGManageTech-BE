using MartyrGraveManagement_BAL.ModelViews.OrdersDetailDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_DAL.Entities;
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
        Task<OrdersGetAllDTOResponse> GetOrderById(int orderId, int managerId);
        Task<(bool status, string? paymentUrl, string responseContent)> CreateOrderFromCartAsync(int accountId, OrdersDTORequest orderBody, string paymentMethod);
        Task<(List<OrdersGetAllDTOResponse> orderList, int totalPage)> GetOrderByAccountId(int accountId, int pageIndex, int pageSize, DateTime? date, int? status);
        Task<OrdersGetAllDTOResponse> GetOrderByIdForCustomer(int orderId, int customerId);
        Task<(List<OrderDetailDtoResponse> orderDetailList, int totalPage)> GetOrderByAreaId(int managerId, int pageIndex, int pageSize, DateTime Date);
        Task<bool> UpdateOrderStatus(int orderId, int newStatus);
        Task<bool> DeleteAsync(int id);

        Task<OrderDetailDtoResponse> GetOrderDetailById(int orderDetailId);
    }
}
