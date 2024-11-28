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
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class OrdersService : IOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;



        public OrdersService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _paymentService = paymentService;
        }


        public async Task<(List<OrdersGetAllDTOResponse> orderList, int totalPage)> GetOrderByAccountId(int accountId, int pageIndex, int pageSize, DateTime? date, int? status)
        {
            try
            {
                int totalPage = 0;
                int totalOrder = 0;
                IEnumerable<Order> orders = new List<Order>();

                // Áp dụng bộ lọc với cả Date và Status
                Expression<Func<Order, bool>> filter = o => o.AccountId == accountId
                    && (!date.HasValue || o.OrderDate.Date == date.Value.Date)
                    && (!status.HasValue || o.Status == status);

                // Đếm tổng số đơn hàng phù hợp với bộ lọc
                totalOrder = (await _unitOfWork.OrderRepository.GetAsync(filter)).Count();
                totalPage = (int)Math.Ceiling(totalOrder / (double)pageSize);

                // Lấy danh sách đơn hàng với phân trang
                orders = await _unitOfWork.OrderRepository.GetAsync(
                    filter: filter,
                    includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations",
                    orderBy: o => o.OrderByDescending(x => x.OrderDate), // Sắp xếp theo ngày mới nhất
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                // Kiểm tra nếu không có đơn hàng nào
                if (orders == null || !orders.Any())
                {
                    return (new List<OrdersGetAllDTOResponse>(), 0);
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
                        int statusTask = task?.AccountId ?? 0;
                        string ImageWorkSpace = task?.ImageWorkSpace;

                        var orderDetailDto = new OrderDetailDtoResponse
                        {
                            OrderId = orderDetail.OrderId,
                            DetailId = orderDetail.DetailId,
                            OrderDate = order.OrderDate,
                            ExpectedCompletionDate = order.ExpectedCompletionDate,
                            Note = order.Note,
                            orderStatus = order.Status,
                            ServiceName = orderDetail.Service?.ServiceName,
                            MartyrName = martyrGraveInfo?.Name,
                            OrderPrice = orderDetail.OrderPrice,
                            StatusTask = statusTask,
                            ImagePath1 = ImageWorkSpace

                        };

                        orderDto.OrderDetails.Add(orderDetailDto);
                    }

                    orderDtoList.Add(orderDto);
                }

                return (orderDtoList, totalPage);
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
                var order = (await _unitOfWork.OrderRepository.GetAsync(
                    filter: o => o.OrderId == orderId,
                    includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations,OrderDetails.MartyrGrave"
                )).FirstOrDefault();

                // Kiểm tra nếu đơn hàng không tồn tại
                if (order == null)
                {
                    return null;  // Hoặc ném lỗi tùy yêu cầu
                }

                // Ánh xạ từ Order sang DTO
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

        public async Task<OrdersGetAllDTOResponse> GetOrderByIdForCustomer(int orderId, int customerId)
        {
            try
            {
                // Fetch order including related entities
                var order = (await _unitOfWork.OrderRepository.GetAsync(
                    filter: o => o.OrderId == orderId,
                    includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations,OrderDetails.MartyrGrave"
                )).FirstOrDefault();

                if (order == null || order.AccountId != customerId)
                {
                    return null;
                }

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

                // Fetch StaffTask separately and join with OrderDetails
                var staffTasks = await _unitOfWork.TaskRepository.GetAsync(t => t.OrderId == orderId, includeProperties: "Account");

                // Map order details and associate Staff data if available
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
                        MartyrName = martyrGraveInfo?.Name,
                        OrderPrice = orderDetail.OrderPrice
                    };

                    // Find related task for this OrderDetail
                    var relatedTask = staffTasks.FirstOrDefault(t => t.DetailId == orderDetail.DetailId);
                    if (relatedTask != null)
                    {
                        orderDetailDto.StatusTask = relatedTask.Status;
                        orderDetailDto.ImagePath1 = relatedTask.ImageWorkSpace;


                        // Map Staff information
                        if (relatedTask.Account != null)
                        {
                            orderDetailDto.Staffs.Add(new StaffDtoResponse
                            {
                                AccountId = relatedTask.Account.AccountId,
                                StaffFullName = relatedTask.Account.FullName
                            });
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


        public async Task<(bool status, string? paymentUrl, string responseContent)> CreateOrderFromCartAsync(int accountId, OrdersDTORequest orderBody, string paymentMethod)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Lấy thông tin tài khoản và giỏ hàng
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                    if (account == null)
                    {
                        return (false, null, "Không tìm thấy tài khoản.");
                    }

                    if (!account.Status)
                    {
                        return (false, null, "Tài khoản đã bị khóa.");
                    }

                    var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == accountId && c.Status == true);
                    if (cartItems == null || !cartItems.Any())
                    {
                        return (false, null, "Không có sản phẩm trong giỏ hàng.");
                    }

                    // Tính toán tổng giá và tạo chi tiết đơn hàng
                    decimal totalPrice = 0;
                    var orderDetails = new List<OrderDetail>();
                    foreach (var cartItem in cartItems)
                    {
                        var service = await _unitOfWork.ServiceRepository.GetByIDAsync(cartItem.ServiceId);
                        if (service == null || !service.Status)
                        {
                            return (false, null, $"Dịch vụ {cartItem.ServiceId} không khả dụng.");
                        }

                        // Lấy thông tin mộ liệt sĩ
                        var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(cartItem.MartyrId);
                        decimal priceToApply = (decimal)service.Price;

                        // Kiểm tra xem người đặt hàng có phải là người quản lý mộ không
                        if (grave.AccountId == account.AccountId)
                        {
                            // Giảm giá 5% nếu người đặt hàng là người quản lý mộ
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

                    // Tạo đơn hàng
                    var order = new Order
                    {
                        AccountId = accountId,
                        OrderDate = DateTime.Now,
                        TotalPrice = totalPrice,
                        Status = 0, // Chưa thanh toán
                        ExpectedCompletionDate = orderBody.ExpectedCompletionDate,
                        Note = orderBody.Note
                    };

                    await _unitOfWork.OrderRepository.AddAsync(order);
                    await _unitOfWork.SaveAsync();

                    // Thêm chi tiết đơn hàng
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.OrderId = order.OrderId;
                        await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
                    }
                    await _unitOfWork.SaveAsync();

                    // Tạo link thanh toán
                    string paymentUrl;
                    if (paymentMethod.ToLower() == "vnpay")
                    {
                        paymentUrl = CreateVnpayLink(order);
                    }
                    else if (paymentMethod.ToLower() == "momo")
                    {
                        var momoPayment = await _paymentService.CreateMomoPayment(order);
                        if (momoPayment == null)
                        {
                            await transaction.RollbackAsync();
                            return (false, null, "Không thể tạo thanh toán MoMo");
                        }
                        paymentUrl = momoPayment.PaymentUrl; // Lấy URL thanh toán từ response
                    }
                    else if (paymentMethod.ToLower() == "balance")
                    {
                        // Lấy ví của khách hàng
                        var customerWallet = (await _unitOfWork.CustomerWalletRepository.GetAsync(
                            w => w.CustomerId == accountId
                        )).FirstOrDefault();

                        if (customerWallet == null)
                        {
                            await transaction.RollbackAsync();
                            return (false, null, "Không tìm thấy ví của khách hàng.");
                        }

                        // Kiểm tra số dư
                        decimal currentBalance = customerWallet.CustomerBalance;
                        if (currentBalance < totalPrice)
                        {
                            await transaction.RollbackAsync();
                            return (false, null, "Số dư không đủ để thực hiện giao dịch này.");
                        }

                        try
                        {
                            // Tính toán số dư mới
                            decimal newBalance = currentBalance - totalPrice;
                            
                            // Tạo Payment record
                            var payment = new Payment
                            {
                                OrderId = order.OrderId,
                                PaymentMethod = "Balance",
                                PaymentInfo = $"Thanh toán đơn hàng {order.OrderId} bằng số dư ví",
                                PayDate = DateTime.Now,
                                TransactionStatus = 1, // Thanh toán thành công
                                PaymentAmount = totalPrice,
                                BankCode = "WALLET", // Thêm giá trị mặc định cho BankCode
                                CardType = "BALANCE", // Thêm giá trị mặc định cho CardType nếu cần
                                TransactionNo = DateTime.Now.Ticks.ToString(), // Tạo số giao dịch unique
                                BankTransactionNo = DateTime.Now.Ticks.ToString() // Tạo số giao dịch ngân hàng unique
                            };
                            await _unitOfWork.PaymentRepository.AddAsync(payment);

                            // Tạo TransactionBalanceHistory
                            var transactionHistory = new TransactionBalanceHistory
                            {
                                CustomerId = accountId,
                                TransactionType = "Payment",
                                Amount = -totalPrice,
                                TransactionDate = DateTime.Now,
                                Description = $"Thanh toán đơn hàng #{order.OrderId}",
                                BalanceAfterTransaction = newBalance
                            };
                            await _unitOfWork.TransactionBalanceHistoryRepository.AddAsync(transactionHistory);

                            // Cập nhật số dư ví
                            customerWallet.CustomerBalance = newBalance;
                            customerWallet.UpdateAt = DateTime.Now;
                            await _unitOfWork.CustomerWalletRepository.UpdateAsync(customerWallet);

                            // Cập nhật trạng thái đơn hàng thành đã thanh toán
                            order.Status = 1; // Đã thanh toán
                            await _unitOfWork.OrderRepository.UpdateAsync(order);

                            // Xóa các mục trong giỏ hàng
                            foreach (var cartItem in cartItems)
                            { 
                                await _unitOfWork.CartItemRepository.DeleteAsync(cartItem);
                            }

                            // Tạo thông báo
                            var notification = new Notification
                            {
                                Title = "Thanh toán đơn hàng thành công",
                                Description = $"Đơn hàng #{order.OrderId} đã được thanh toán thành công.\n" +
                                             $"Số tiền thanh toán: {totalPrice:N0} VNĐ\n" +
                                             $"Số dư ban đầu: {currentBalance:N0} VNĐ\n" +
                                             $"Số dư còn lại: {newBalance:N0} VNĐ",
                                CreatedDate = DateTime.Now,
                                Status = true
                            };
                            await _unitOfWork.NotificationRepository.AddAsync(notification);
                            await _unitOfWork.SaveAsync();

                            // Liên kết thông báo với tài khoản
                            var notificationAccount = new NotificationAccount
                            {
                                AccountId = accountId,
                                NotificationId = notification.NotificationId,
                                Status = true
                            };
                            await _unitOfWork.NotificationAccountsRepository.AddAsync(notificationAccount);

                            await _unitOfWork.SaveAsync();
                            await transaction.CommitAsync();
                            return (true, null, "Đơn hàng đã được thanh toán thành công bằng số dư tài khoản.");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            throw new Exception($"Lỗi khi xử lý thanh toán: {ex.Message}");
                        }
                    }
                    else
                    {
                        return (false, null, "Phương thức thanh toán không hợp lệ.");
                    }

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

        public async Task<(List<OrderDetailDtoResponse> orderDetailList, int totalPage)> GetOrderByAreaId(int managerId, int pageIndex, int pageSize, DateTime Date)
        {
            try
            {
                // Lấy các đơn hàng theo AreaId, bao gồm các chi tiết liên quan
                //var orders = await _unitOfWork.OrderRepository.GetAsync(
                //    filter: o => o.OrderDetails.Any(od => od.MartyrGrave.AreaId == areaId),
                //    includeProperties: "OrderDetails,OrderDetails.Service,OrderDetails.MartyrGrave.MartyrGraveInformations"
                //);
                var manager = await _unitOfWork.AccountRepository.GetByIDAsync(managerId);
                if (manager == null)
                {
                    return (new List<OrderDetailDtoResponse>(), 0); // Trả về danh sách rỗng nếu không có đơn hàng
                }
                int totalPage = 0;
                int totalOrder = 0;
                IEnumerable<OrderDetail> orderDetails = new List<OrderDetail>();
                if (Date == DateTime.MinValue)
                {
                    totalOrder = (await _unitOfWork.OrderDetailRepository.GetAsync(s => s.MartyrGrave.AreaId == manager.AreaId,
                    includeProperties: "MartyrGrave,Order")).Count();
                    totalPage = (int)Math.Ceiling(totalOrder / (double)pageSize);
                    orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.MartyrGrave.AreaId == manager.AreaId,
                    includeProperties: "MartyrGrave.MartyrGraveInformations,Service,Order", orderBy: q => q.OrderByDescending(s => s.Order.OrderDate), pageIndex: pageIndex, pageSize: pageSize);
                }
                else
                {
                    totalOrder = (await _unitOfWork.OrderDetailRepository.GetAsync(s => s.MartyrGrave.AreaId == manager.AreaId && s.Order.OrderDate.Date == Date.Date,
                    includeProperties: "MartyrGrave,Order")).Count();
                    totalPage = (int)Math.Ceiling(totalOrder / (double)pageSize);
                    orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.MartyrGrave.AreaId == manager.AreaId && od.Order.OrderDate.Date == Date.Date,
                    includeProperties: "MartyrGrave.MartyrGraveInformations,Service,Order", pageIndex: pageIndex, pageSize: pageSize);
                }



                // Kiểm tra nếu không có đơn hàng cho AreaId này
                //if (orderDetail == null || !orderDetail.Any())
                //{
                //    return new List<OrdersGetAllDTOResponse>(); // Trả về danh sách rỗng nếu không có đơn hàng
                //}
                // Kiểm tra nếu đơn hàng không tồn tại
                var orderEntity = orderDetails.FirstOrDefault();
                if (orderEntity == null)
                {
                    return (new List<OrderDetailDtoResponse>(), 0); // Trả về danh sách rỗng nếu không có đơn hàng
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

                return (orderDetailList, totalPage);
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

            // To đối tượng DTO với các thông tin yêu cầu
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

            if (orderDetailDto.StatusTask == 1 || orderDetailDto.StatusTask == 3 || orderDetailDto.StatusTask == 4 || orderDetailDto.StatusTask == 5)
            {
                // Lấy danh sách nhân viên thuộc cùng AreaId và task của detail đó
                var taskStaff = (await _unitOfWork.TaskRepository.GetAsync(t => t.DetailId == orderDetailDto.DetailId, includeProperties: "Account")).FirstOrDefault();
                if (taskStaff != null)
                {
                    var orderDetailStaff = new StaffDtoResponse
                    {
                        AccountId = taskStaff.AccountId,
                        StaffFullName = taskStaff.Account.FullName
                    };
                    orderDetailDto.Staffs.Add(orderDetailStaff);
                }
            }
            else
            {
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
