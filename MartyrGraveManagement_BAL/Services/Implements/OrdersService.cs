using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDetailDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using MartyrGraveManagement_BAL.ModelViews.StaffDTOs;
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
    public class OrdersService : IOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public OrdersService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<List<OrdersGetAllDTOResponse>> GetOrderByAccountId(int accountId)
        {
            try
            {
                // Lấy tất cả các đơn hàng dựa trên AccountId và bao gồm các chi tiết đơn hàng
                var orders = await _unitOfWork.OrderRepository.GetAsync(
                    filter: o => o.AccountId == accountId,
                    includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations"
                );

                // Kiểm tra nếu không có đơn hàng nào cho AccountId này
                if (orders == null || !orders.Any())
                {
                    return new List<OrdersGetAllDTOResponse>();  // Trả về danh sách rỗng nếu không có đơn hàng
                }

                // Ánh xạ từng đơn hàng và chi tiết đơn hàng sang DTO
                var orderDtoList = new List<OrdersGetAllDTOResponse>();

                foreach (var order in orders)
                {
                    var orderDto = new OrdersGetAllDTOResponse
                    {
                        OrderId = order.OrderId,
                        AccountId = order.AccountId,
                        OrderDate = order.OrderDate,
                        ExpectedCompletionDate = order.ExpectedCompletionDate,
                        Note = order.Note,
                        TotalPrice = order.TotalPrice,
                        Status = order.Status
                    };

                    // Ánh xạ chi tiết đơn hàng
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var martyrGraveInfo = orderDetail.MartyrGrave?.MartyrGraveInformations?.FirstOrDefault();
                        var task = (await _unitOfWork.TaskRepository.GetAsync(t => t.DetailId == orderDetail.DetailId)).FirstOrDefault();
                        int statusTask = 0;
                        if (task != null)
                        {
                            statusTask = task.Status;
                        }
                        var orderDetailDto = new OrderDetailDtoResponse
                        {
                            OrderId = orderDetail.OrderId,
                            DetailId = orderDetail.DetailId,
                            OrderDate = order.OrderDate,
                            ExpectedCompletionDate = order.ExpectedCompletionDate,
                            Note = order.Note,
                            orderStatus = order.Status,
                            ServiceName = orderDetail.Service?.ServiceName,
                            MartyrName = martyrGraveInfo?.Name,  // Lấy thông tin liệt sĩ từ MartyrGraveInformation
                            OrderPrice = orderDetail.OrderPrice,
                            StatusTask = statusTask
                        };

                        orderDto.OrderDetails.Add(orderDetailDto);
                    }

                    // Thêm DTO của đơn hàng vào danh sách kết quả
                    orderDtoList.Add(orderDto);
                }

                return orderDtoList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<List<OrdersGetAllDTOResponse>> GetAllOrders()
        {
            try
            {
                // Lấy tất cả các đơn hàng cùng với chi tiết đơn hàng và các thuộc tính liên quan
                var orders = await _unitOfWork.OrderRepository.GetAsync(includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations");

                // Kiểm tra nếu không có đơn hàng nào
                if (orders == null || !orders.Any())
                {
                    return new List<OrdersGetAllDTOResponse>();  // Trả về danh sách rỗng nếu không có đơn hàng
                }

                // Ánh xạ từng đơn hàng và chi tiết đơn hàng sang DTO
                var orderDtoList = new List<OrdersGetAllDTOResponse>();

                foreach (var order in orders)
                {
                    var orderDto = new OrdersGetAllDTOResponse
                    {
                        OrderId = order.OrderId,
                        AccountId = order.AccountId,
                        OrderDate = order.OrderDate,
                        ExpectedCompletionDate = order.ExpectedCompletionDate,
                        Note = order.Note,
                        TotalPrice = order.TotalPrice,
                        Status = order.Status
                    };

                    // Ánh xạ chi tiết đơn hàng
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var martyrGraveInfo = orderDetail.MartyrGrave?.MartyrGraveInformations?.FirstOrDefault();

                        var orderDetailDto = new OrderDetailDtoResponse
                        {
                            OrderId = orderDetail.OrderId,
                            DetailId = orderDetail.DetailId,
                            OrderDate = order.OrderDate,
                            ExpectedCompletionDate = order.ExpectedCompletionDate,
                            Note = order.Note,
                            orderStatus = order.Status,
                            ServiceName = orderDetail.Service?.ServiceName,
                            MartyrName = martyrGraveInfo?.Name, // Lấy thông tin liệt sĩ từ MartyrGraveInformation
                            OrderPrice = orderDetail.OrderPrice
                        };

                        orderDto.OrderDetails.Add(orderDetailDto);
                    }

                    // Thêm DTO của đơn hàng vào danh sách kết quả
                    orderDtoList.Add(orderDto);
                }

                return orderDtoList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrdersGetAllDTOResponse> GetOrderById(int orderId, int managerId)
        {
            try
            {
                // Lấy đơn hàng dựa trên OrderId và bao gồm các chi tiết liên quan
                var order = await _unitOfWork.OrderRepository.GetAsync(
                    filter: o => o.OrderId == orderId,
                    includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations,OrderDetails.MartyrGrave"
                );

                // Kiểm tra nếu đơn hàng không tồn tại
                var orderEntity = order.FirstOrDefault();
                if (orderEntity == null)
                {
                    return null;  // Hoặc ném lỗi tùy yêu cầu
                }

                // Ánh xạ từ Order sang DTO
                var orderDto = new OrdersGetAllDTOResponse
                {
                    OrderId = orderEntity.OrderId,
                    AccountId = orderEntity.AccountId,
                    OrderDate = orderEntity.OrderDate,
                    ExpectedCompletionDate = orderEntity.ExpectedCompletionDate,
                    Note = orderEntity.Note,
                    TotalPrice = orderEntity.TotalPrice,
                    Status = orderEntity.Status
                };

                // Ánh xạ chi tiết đơn hàng
                foreach (var orderDetail in orderEntity.OrderDetails)
                {
                    var martyrGraveInfo = orderDetail.MartyrGrave?.MartyrGraveInformations?.FirstOrDefault();

                    var orderDetailDto = new OrderDetailDtoResponse
                    {
                        OrderId = orderDetail.OrderId,
                        DetailId = orderDetail.DetailId,
                        OrderDate = orderEntity.OrderDate,
                        ExpectedCompletionDate = orderEntity.ExpectedCompletionDate,
                        Note = orderEntity.Note,
                        orderStatus = orderEntity.Status,
                        ServiceName = orderDetail.Service?.ServiceName,
                        MartyrName = martyrGraveInfo?.Name,
                        OrderPrice = orderDetail.OrderPrice
                    };

                    var taskStatus = (await _unitOfWork.TaskRepository.GetAsync(t => t.DetailId == orderDetail.DetailId)).FirstOrDefault();
                    if (taskStatus != null)
                    {
                        orderDetailDto.StatusTask = taskStatus.Status;
                    }
                    var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                    if (manager.AreaId == orderDetail.MartyrGrave.AreaId)
                    {
                        var accountStaffs = await _unitOfWork.AccountRepository.GetAsync(s => s.AreaId == orderDetail.MartyrGrave.AreaId && s.RoleId == 3);
                        if (accountStaffs != null)
                        {
                            foreach (var accountStaff in accountStaffs)
                            {
                                if (accountStaff.Status == true)
                                {
                                    var staffDto = new StaffDtoResponse
                                    {
                                        AccountId = accountStaff.AccountId,
                                        StaffFullName = accountStaff.FullName
                                    };
                                    orderDetailDto.Staffs?.Add(staffDto);
                                }
                            }
                        }
                    }
                    orderDto.OrderDetails.Add(orderDetailDto);
                }

                return orderDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving order: {ex.Message}");
            }
        }


        //public async Task<(bool status, string? paymentUrl, string responseContent)> CreateOrderFromCartAsync(int accountId)
        //{
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // Kiểm tra xem AccountID có tồn tại không
        //            var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
        //            if (account == null)
        //            {
        //                return (false, null, "Không tìm thấy account, kiểm tra lại");
        //            }

        //            // Kiểm tra trạng thái tài khoản
        //            if (account.Status == false)
        //            {
        //                return (false, null, "Account đã bị lock");
        //            }

        //            // Lấy danh sách CartItem cho account
        //            var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == accountId && c.Status == true);

        //            if (cartItems == null || !cartItems.Any())
        //            {
        //                return (false, null, "Không có item trong cart");
        //            }

        //            //// Kiểm tra nếu người dùng đã có đơn hàng chưa thanh toán
        //            //var existingOrder = await _unitOfWork.OrderRepository.GetAsync(o => o.AccountId == accountId && o.Status == 0);
        //            //if (existingOrder.Any())
        //            //{
        //            //    return (false, null, "Bạn có đơn hàng chưa thanh toán");
        //            //}

        //            // Tính tổng tiền dựa trên dịch vụ trong giỏ hàng
        //            decimal totalPrice = 0;
        //            List<OrderDetail> orderDetails = new List<OrderDetail>();

        //            foreach (var cartItem in cartItems)
        //            {
        //                var service = await _unitOfWork.ServiceRepository.GetByIDAsync(cartItem.ServiceId);
        //                if (service != null)
        //                {
        //                    // Kiểm tra trạng thái của dịch vụ
        //                    if (service.Status == false)
        //                    {
        //                        return (false, null, "Dịch vụ đã bị dừng");
        //                    }

        //                    totalPrice += (decimal)service.Price;  // Sử dụng giá của dịch vụ
        //                    var orderDetail = new OrderDetail
        //                    {
        //                        ServiceId = cartItem.ServiceId,
        //                        MartyrId = cartItem.MartyrId,
        //                        OrderPrice = service.Price,
        //                        Status = true
        //                    };
        //                    orderDetails.Add(orderDetail);
        //                }
        //            }

        //            // Tạo Order mới từ CartItem
        //            var order = new Order
        //            {
        //                AccountId = accountId,
        //                OrderDate = DateTime.Now,
        //                StartDate = DateTime.Now,  // Hoặc dựa trên yêu cầu cụ thể
        //                TotalPrice = totalPrice,
        //                Status = 0,  // Status = 0 cho đơn hàng chưa thanh toán
        //            };
        //            order.EndDate = order.StartDate.AddDays(7);  // Ví dụ thêm 7 ngày cho thời gian hết hạn

        //            // Thêm Order vào cơ sở dữ liệu
        //            await _unitOfWork.OrderRepository.AddAsync(order);
        //            await _unitOfWork.SaveAsync();

        //            // Thêm OrderDetail
        //            foreach (var orderDetail in orderDetails)
        //            {
        //                orderDetail.OrderId = order.OrderId; // Gán OrderId cho các chi tiết
        //                orderDetail.DetailId = 0;
        //                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
        //            }
        //            await _unitOfWork.SaveAsync();


        //            // Xóa các mục trong giỏ hàng sau khi tạo đơn hàng
        //            foreach (var cartItem in cartItems)
        //            {
        //                await _unitOfWork.CartItemRepository.DeleteAsync(cartItem);
        //            }


        //            // Tạo liên kết thanh toán VNPay
        //            var paymentUrl = CreateVnpayLink(order);

        //            // Commit transaction
        //            await transaction.CommitAsync();



        //            return (true, paymentUrl, "Đơn hàng đã được tạo thành công, hãy thanh toán đơn hàng với đường link đính kèm để hoàn thành thanh toán");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback nếu có lỗi
        //            await transaction.RollbackAsync();
        //            throw new Exception(ex.Message);
        //        }
        //    }
        //}


        public async Task<(bool status, string? paymentUrl, string responseContent)> CreateOrderFromCartAsync(int accountId, OrdersDTORequest orderBody)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                    if (account == null)
                    {
                        return (false, null, "Không tìm thấy tài khoản.");
                    }

                    if (account.Status == false)
                    {
                        return (false, null, "Tài khoản đã bị khóa.");
                    }

                    var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == accountId && c.Status == true);
                    if (cartItems == null || !cartItems.Any())
                    {
                        return (false, null, "Không có sản phẩm trong giỏ hàng.");
                    }

                    // Kiểm tra tất cả dịch vụ trước khi thêm vào đơn hàng
                    decimal totalPrice = 0;
                    List<OrderDetail> orderDetails = new List<OrderDetail>();

                    foreach (var cartItem in cartItems)
                    {
                        var service = await _unitOfWork.ServiceRepository.GetByIDAsync(cartItem.ServiceId);
                        if (service == null || service.Status == false)
                        {
                            return (false, null, $"Dịch vụ {cartItem.ServiceId} đã ngừng cung cấp.");
                        }

                        // Kiểm tra nếu CustomerCode của Account và MartyrGrave trùng nhau thì áp dụng giảm giá
                        var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(cartItem.MartyrId);
                        decimal priceToApply = (decimal)service.Price;

                        if (grave.AccountId == account.AccountId)
                        {
                            // Giảm giá 5% nếu điều kiện CustomerCode trùng khớp
                            priceToApply *= 0.95m;
                        }

                        totalPrice += priceToApply;
                        orderDetails.Add(new OrderDetail
                        {
                            ServiceId = cartItem.ServiceId,
                            MartyrId = cartItem.MartyrId,
                            OrderPrice = (double)priceToApply,
                            Status = true
                        });
                    }

                    // Tạo Order sau khi kiểm tra
                    var order = new Order
                    {
                        AccountId = accountId,
                        OrderDate = DateTime.Now,
                        TotalPrice = totalPrice,
                        Status = 0,  // Chưa thanh toán
                        ExpectedCompletionDate = orderBody.ExpectedCompletionDate,
                        Note = orderBody.Note
                    };

                    await _unitOfWork.OrderRepository.AddAsync(order);
                    await _unitOfWork.SaveAsync();

                    // Thêm các chi tiết đơn hàng
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.OrderId = order.OrderId;
                        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
                    }
                    await _unitOfWork.SaveAsync();

                    // Tạo link thanh toán VNPay
                    var paymentUrl = CreateVnpayLink(order);
                    await transaction.CommitAsync();

                    return (true, paymentUrl, "Đơn hàng đã được tạo thành công. Vui lòng thanh toán.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }





        public async Task<bool> UpdateOrderStatus(int orderId, int newStatus)
        {
            try
            {
                // Lấy đơn hàng dựa trên OrderId
                var order = await _unitOfWork.OrderRepository.GetByIDAsync(orderId);

                // Kiểm tra nếu đơn hàng không tồn tại
                if (order == null)
                {
                    throw new KeyNotFoundException("Order not found.");
                }

                // Cập nhật trạng thái của đơn hàng
                order.Status = newStatus;

                // Cập nhật thông tin vào cơ sở dữ liệu
                await _unitOfWork.OrderRepository.UpdateAsync(order);

                return true;  // Trả về true khi cập nhật thành công
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the order status: {ex.Message}");
            }
        }

        public async Task<List<OrderDetailDtoResponse>> GetOrderByAreaId(int managerId)
        {
            try
            {
                // Lấy các đơn hàng theo AreaId, bao gồm các chi tiết liên quan
                //var orders = await _unitOfWork.OrderRepository.GetAsync(
                //    filter: o => o.OrderDetails.Any(od => od.MartyrGrave.AreaId == areaId),
                //    includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations"
                //);
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if(manager == null)
                {
                    return new List<OrderDetailDtoResponse>(); // Trả về danh sách rỗng nếu không có đơn hàng
                }
                var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.MartyrGrave.AreaId == manager.AreaId, includeProperties: "MartyrGrave.MartyrGraveInformations,Service,Order");


                // Kiểm tra nếu không có đơn hàng cho AreaId này
                //if (orderDetail == null || !orderDetail.Any())
                //{
                //    return new List<OrdersGetAllDTOResponse>(); // Trả về danh sách rỗng nếu không có đơn hàng
                //}
                // Kiểm tra nếu đơn hàng không tồn tại
                var orderEntity = orderDetails.FirstOrDefault();
                if (orderEntity == null)
                {
                    return new List<OrderDetailDtoResponse>(); // Trả về danh sách rỗng nếu không có đơn hàng
                }


                var orderDetailList = new List<OrderDetailDtoResponse>();

                foreach (var orderDetail in orderDetails)
                {
                        var martyrGraveInfo = orderDetail.MartyrGrave?.MartyrGraveInformations?.FirstOrDefault();

                        var orderDetailDto = new OrderDetailDtoResponse
                        {
                            OrderId = orderDetail.OrderId,
                            DetailId = orderDetail.DetailId,
                            OrderDate = orderDetail.Order.OrderDate,
                            ExpectedCompletionDate = orderDetail.Order.ExpectedCompletionDate,
                            Note = orderDetail.Order.Note,
                            orderStatus = orderDetail.Order.Status,
                            ServiceName = orderDetail.Service?.ServiceName,
                            MartyrName = martyrGraveInfo?.Name, // Lấy thông tin liệt sĩ từ MartyrGraveInformation
                            OrderPrice = orderDetail.OrderPrice
                        };

                        var taskStatus = (await _unitOfWork.TaskRepository.GetAsync(t => t.DetailId == orderDetail.DetailId)).FirstOrDefault();
                        if (taskStatus != null)
                        {
                            orderDetailDto.StatusTask = taskStatus.Status;
                        }
                        if (manager.AreaId == orderDetail.MartyrGrave.AreaId)
                        {
                            var accountStaffs = await _unitOfWork.AccountRepository.GetAsync(s => s.AreaId == orderDetail.MartyrGrave.AreaId && s.RoleId == 3);
                            if (accountStaffs != null)
                            {
                                foreach (var accountStaff in accountStaffs)
                                {
                                    if (accountStaff.Status == true)
                                    {
                                        var staffDto = new StaffDtoResponse
                                        {
                                            AccountId = accountStaff.AccountId,
                                            StaffFullName = accountStaff.FullName
                                        };
                                        orderDetailDto.Staffs?.Add(staffDto);
                                    }
                                }
                            }
                        }
                        orderDetailList.Add(orderDetailDto);


                    
                }

                return orderDetailList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<OrderDetailDtoResponse> GetOrderDetailById(int detailId)
        {
            // Truy vấn dữ liệu OrderDetail với các thuộc tính liên quan
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(
                filter: od => od.DetailId == detailId,
                includeProperties: "Order,Service,MartyrGrave.MartyrGraveInformations,StaffTask"
            );

            // Lấy đối tượng OrderDetail đầu tiên hoặc trả về null nếu không tồn tại
            var orderDetail = orderDetails.FirstOrDefault();
            if (orderDetail == null)
            {
                return null;
            }

            // Tạo đối tượng DTO với các thông tin yêu cầu
            var martyrGraveInfo = orderDetail.MartyrGrave?.MartyrGraveInformations?.FirstOrDefault();
            var orderDetailDto = new OrderDetailDtoResponse
            {
                OrderId = orderDetail.OrderId,
                DetailId = orderDetail.DetailId,
                OrderDate = orderDetail.Order.OrderDate,
                ExpectedCompletionDate = orderDetail.Order.ExpectedCompletionDate,
                Note = orderDetail.Order.Note,
                orderStatus = orderDetail.Order.Status,
                ServiceName = orderDetail.Service?.ServiceName,
                MartyrName = martyrGraveInfo?.Name,
                OrderPrice = orderDetail.OrderPrice,
                StatusTask = orderDetail.StaffTask?.Status ?? 0
            };

            // Lấy danh sách nhân viên thuộc cùng AreaId
            var accountStaffs = await _unitOfWork.AccountRepository.GetAsync(
                s => s.AreaId == orderDetail.MartyrGrave.AreaId && s.RoleId == 3 && s.Status == true
            );

            // Thêm danh sách nhân viên vào DTO
            if (accountStaffs != null && accountStaffs.Any())
            {
                orderDetailDto.Staffs = accountStaffs.Select(accountStaff => new StaffDtoResponse
                {
                    AccountId = accountStaff.AccountId,
                    StaffFullName = accountStaff.FullName
                }).ToList();
            }

            return orderDetailDto;
        }






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
