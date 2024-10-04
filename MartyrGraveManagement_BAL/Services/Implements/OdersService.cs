using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_BAL.VNPay;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class OdersService : IOdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public OdersService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<IEnumerable<OrdersDTOResponse>> GetAll()
        {
            var items = await _unitOfWork.OrderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrdersDTOResponse>>(items);
        }
        public async Task<IEnumerable<OrdersDTOResponse>> GetById(int id)
        {
            var items = await _unitOfWork.OrderRepository.GetByIDAsync(id);
            return _mapper.Map<IEnumerable<OrdersDTOResponse>>(items);
        }

        public async Task<OrdersDTOResponse> CreateOrderFromCartAsync(int accountId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra xem AccountID có tồn tại không
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                    if (account == null)
                    {
                        throw new KeyNotFoundException("AccountID does not exist.");
                    }

                    // Lấy danh sách CartItem cho account
                    var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == accountId && c.Status == true);

                    if (cartItems == null || !cartItems.Any())
                    {
                        throw new Exception("No items in cart to process.");
                    }

                    // Tính tổng tiền dựa trên dịch vụ trong giỏ hàng
                    decimal totalPrice = 0;
                    List<OrderDetail> orderDetails = new List<OrderDetail>();

                    foreach (var cartItem in cartItems)
                    {
                        var service = await _unitOfWork.ServiceRepository.GetByIDAsync(cartItem.ServiceId);
                        if (service != null)
                        {
                            totalPrice += (decimal)service.Price;  // Sử dụng giá của dịch vụ
                            var orderDetail = new OrderDetail
                            {
                                ServiceId = cartItem.ServiceId,
                                MartyrId = cartItem.MartyrId,
                                OrderPrice = service.Price,
                                Status = true
                            };
                            orderDetails.Add(orderDetail);
                        }
                    }

                    // Tạo Order mới từ CartItem
                    var order = new Order
                    {
                        AccountId = accountId,
                        OrderDate = DateTime.Now,
                        StartDate = DateTime.Now,  // Hoặc dựa trên yêu cầu cụ thể
                        TotalPrice = totalPrice,
                        Status = 0,
                        OrderDetails = orderDetails
                    };
                    order.EndDate = order.StartDate.AddDays(7);

                    // Thêm Order vào cơ sở dữ liệu

                    await _unitOfWork.OrderRepository.AddAsync(order);
                    await _unitOfWork.SaveAsync();

                    // Thêm OrderDetail
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.OrderId = order.OrderId; // Gán OrderId cho các chi tiết
                        orderDetail.DetailId = 0;
                        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
                    }
                    await _unitOfWork.SaveAsync();

                    // Xóa các mục trong giỏ hàng sau khi tạo đơn hàng
                    foreach (var cartItem in cartItems)
                    {
                        await _unitOfWork.CartItemRepository.DeleteAsync(cartItem);
                    }
                    await _unitOfWork.SaveAsync();

                    // Tạo liên kết thanh toán VNPay
                    var paymentUrl = CreateVnpayLink(order);

                    // Commit transaction
                    await transaction.CommitAsync();

                    // Trả về DTO response với Payment URL
                    var response = _mapper.Map<OrdersDTOResponse>(order);
                    response.PaymentUrl = paymentUrl;

                    return response;
                }
                catch (Exception ex)
                {
                    // Rollback nếu có lỗi
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }




        //public async Task<OrdersDTOResponse> UpdateAsync(int id, OrdersDTORequest ordersDTO)
        //{
        //    // Kiểm tra AreaId có tồn tại không
        //    var account = await _unitOfWork.AreaRepository.GetByIDAsync(ordersDTO.AccountId);
        //    if (account == null)
        //    {
        //        throw new KeyNotFoundException("AccountID does not exist.");
        //    }
            

        //    var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
        //    if (order == null)
        //    {
        //        return null;
        //    }

        //    // Cập nhật các thuộc tính từ DTO sang thực thể
        //    _mapper.Map(ordersDTO, order);

        //    // Cập nhật thông tin vào cơ sở dữ liệu
        //    await _unitOfWork.OrderRepository.UpdateAsync(order);
        //    await _unitOfWork.SaveAsync();

        //    // Trả về kết quả cập nhật
        //    return _mapper.Map<OrdersDTOResponse>(order);
        //}

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
            if (order == null)
            {
                return false;
            }
            // Cập nhật tình trạng
            order.Status = 0;

            //Cập nhật thông tin vào cơ sở dữ liệu
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.SaveAsync();
            return true;
        }


        private string CreateVnpayLink(Order order)
        {
            var paymentUrl = string.Empty;

            var vpnRequest = new VNPayRequest(_configuration["VNpay:Version"], _configuration["VNpay:tmnCode"],
                order.OrderDate, "https://localhost:7006", (decimal)order.TotalPrice, "VND", "other",
                $"Thanh toan don hang {order.OrderId}", _configuration["VNpay:ReturnUrl"],
                $"{order.OrderId}");

            paymentUrl = vpnRequest.GetLink(_configuration["VNpay:PaymentUrl"],
                _configuration["VNpay:HashSecret"]);

            return paymentUrl;
        }
    }
}
