using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CustomerWalletDTOs;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_BAL.Utils;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork, ITaskService taskService, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _taskService = taskService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<PaymentDTOResponse> CancelTransaction(PaymentDTORequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedOrder = await _unitOfWork.OrderRepository.GetByIDAsync(int.Parse(paymentRequest.vnp_TxnRef));
                    if (existedOrder != null)
                    {
                        var existedPayment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == existedOrder.OrderId);
                        if (existedPayment.Any())
                        {
                            return null;
                        }
                        var payment = new Payment()
                        {
                            PaymentMethod = "VNPay",
                            BankCode = paymentRequest.vnp_BankCode,

                            BankTransactionNo = paymentRequest.vnp_BankTranNo,

                            CardType = paymentRequest.vnp_CardType,
                            PaymentInfo = paymentRequest.vnp_OrderInfo,
                            PayDate = DateTime.ParseExact(paymentRequest.vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            TransactionNo = paymentRequest.vnp_TransactionNo,
                            TransactionStatus = int.Parse(paymentRequest.vnp_TransactionStatus),
                            PaymentAmount = decimal.Parse(paymentRequest.vnp_Amount) / 100,
                            OrderId = int.Parse(paymentRequest.vnp_TxnRef)
                        };
                        await _unitOfWork.PaymentRepository.AddAsync(payment);

                        // Update Order's status is Cancelled
                        existedOrder.Status = 2;
                        await _unitOfWork.OrderRepository.UpdateAsync(existedOrder);

                        // return back quantity to product
                        var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync(o => o.OrderId == existedOrder.OrderId);
                        foreach (var od in orderDetails)
                        {
                            var product = await _unitOfWork.ServiceRepository.GetByIDAsync(od.ServiceId);

                            //product.ProductQuantity += od.OrderQuantity;

                            await _unitOfWork.ServiceRepository.UpdateAsync(product);
                        }

                        // return points customer point if used
                        /*if (existedOrder.ExchangedPoint > 0)
                        {
                            var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(existedOrder.CustomerId);
                            customer.Point += existedOrder.ExchangedPoint;
                            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
                        }*/

                        // return voucher to shop if used
                        /*if (existedOrder.VoucherId != null)
                        {
                            var voucher = await _unitOfWork.VoucherOfShopRepository.GetByIDAsync(existedOrder.VoucherId);
                            voucher.VoucherQuantity++;
                            await _unitOfWork.VoucherOfShopRepository.UpdateAsync(voucher);
                        }*/

                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return _mapper.Map<PaymentDTOResponse>(payment);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<PaymentDTOResponse> CreatePayment(PaymentDTORequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var existedOrder = await _unitOfWork.OrderRepository.GetByIDAsync(
                        paymentRequest.vnp_BankTranNo != null
                            ? long.Parse(paymentRequest.vnp_TxnRef)
                            : long.Parse(paymentRequest.orderId));

                    if (existedOrder == null)
                    {
                        return null;
                    }

                    var existedPayment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == existedOrder.OrderId);
                    if (existedPayment.Any())
                    {
                        return null;
                    }

                    Payment payment;
                    if (paymentRequest.vnp_BankTranNo != null) // VNPay payment
                    {
                        payment = new Payment
                        {
                            PaymentMethod = "VNPay",
                            BankCode = paymentRequest.vnp_BankCode,
                            BankTransactionNo = paymentRequest.vnp_BankTranNo,
                            CardType = paymentRequest.vnp_CardType,
                            PaymentInfo = paymentRequest.vnp_OrderInfo,
                            PayDate = DateTime.ParseExact(paymentRequest.vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                            TransactionNo = paymentRequest.vnp_TransactionNo,
                            TransactionStatus = int.Parse(paymentRequest.vnp_TransactionStatus),
                            PaymentAmount = decimal.Parse(paymentRequest.vnp_Amount) / 100,
                            OrderId = long.Parse(paymentRequest.vnp_TxnRef)
                        };
                    }
                    else // MoMo payment
                    {
                        payment = new Payment
                        {
                            PaymentMethod = "MoMo",
                            BankCode = "MOMO",
                            CardType = "QR",
                            PaymentInfo = paymentRequest.orderInfo,
                            PayDate = DateTime.Now,
                            TransactionNo = paymentRequest.requestId,
                            BankTransactionNo = paymentRequest.transId?.ToString(),
                            TransactionStatus = int.Parse(paymentRequest.resultCode),
                            PaymentAmount = decimal.Parse(paymentRequest.amount),
                            OrderId = long.Parse(paymentRequest.orderId)
                        };
                    }

                    await _unitOfWork.PaymentRepository.AddAsync(payment);

                    // Cập nhật trạng thái đơn hàng và xóa giỏ hàng chỉ khi thanh toán thành công
                    if ((payment.PaymentMethod == "VNPay" && payment.TransactionStatus == 00) ||
                        (payment.PaymentMethod == "MoMo" && payment.TransactionStatus == 0))
                    {
                        existedOrder.Status = 1; // Đã thanh toán
                        await _unitOfWork.OrderRepository.UpdateAsync(existedOrder);

                        var cartItems = await _unitOfWork.CartItemRepository.GetAsync(
                            c => c.AccountId == existedOrder.AccountId && c.Status == true);
                        foreach (var cartItem in cartItems)
                        {
                            await _unitOfWork.CartItemRepository.DeleteAsync(cartItem);
                        }

                        // Tạo thông báo sau khi thanh toán thành công
                        await CreateNotification(
                            "Thanh toán đơn hàng thành công",
                            $"Đơn hàng #{existedOrder.OrderId} đã được thanh toán thành công với số tiền {payment.PaymentAmount:N0} VNĐ qua {payment.PaymentMethod}.",
                            existedOrder.AccountId, $"/order-detail-cus/{existedOrder.OrderId}"
                        );

                        // Lấy danh sách OrderDetail để tạo công việc cho nhân viên
                        var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == existedOrder.OrderId);
                        var taskRequests = orderDetails.Select(od => new TaskDtoRequest
                        {
                            OrderId = existedOrder.OrderId,
                            DetailId = od.DetailId
                        }).ToList();

                        // Gọi hàm tạo công việc tự động
                        await _taskService.CreateTasksAsync(taskRequests);
                    }

                    await transaction.CommitAsync();

                    return _mapper.Map<PaymentDTOResponse>(payment);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error creating payment: {ex.Message}");
                }
            }
        }




        public async Task<List<PaymentDTOResponseForAdmin>> GetPaymentList(DateTime startDate, DateTime endDate, int? status)
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetAsync(p => DateOnly.FromDateTime(p.PayDate.Date) >= DateOnly.FromDateTime(startDate.AddDays(1)) && DateOnly.FromDateTime(p.PayDate.Date) <= DateOnly.FromDateTime(endDate.AddDays(1)));

                if (payments != null && payments.Any())
                {
                    var paymentList = new List<PaymentDTOResponseForAdmin>();
                    foreach (var payment in payments)
                    {
                        if (status != 0)
                        {
                            var order = (await _unitOfWork.OrderRepository.FindAsync(p => p.OrderId == payment.OrderId && p.Status == status)).FirstOrDefault();
                            if (order != null)
                            {
                                var customer = await _unitOfWork.AccountRepository.GetByIDAsync(order.AccountId);
                                if (customer != null)
                                {
                                    var paymentDtoResponse = new PaymentDTOResponseForAdmin
                                    {
                                        OrderId = payment.OrderId,
                                        PaymentMethod = payment.PaymentMethod,
                                        BankCode = payment.BankCode,
                                        CardType = payment.CardType,
                                        CustomerName = customer.FullName,
                                        PayDate = payment.PayDate,
                                        Status = payment.TransactionStatus,
                                        PaymentAmount = payment.PaymentAmount,
                                    };
                                    paymentList.Add(paymentDtoResponse);
                                }
                                else
                                {
                                    throw new KeyNotFoundException("Customer not found");
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            var order = (await _unitOfWork.OrderRepository.FindAsync(p => p.OrderId == payment.OrderId)).FirstOrDefault();
                            if (order != null)
                            {
                                var customer = await _unitOfWork.AccountRepository.GetByIDAsync(order.AccountId);
                                if (customer != null)
                                {
                                    var paymentDtoResponse = new PaymentDTOResponseForAdmin
                                    {
                                        OrderId = payment.OrderId,
                                        PaymentMethod = payment.PaymentMethod,
                                        BankCode = payment.BankCode,
                                        CardType = payment.CardType,
                                        CustomerName = customer.FullName,
                                        PayDate = payment.PayDate,
                                        Status = payment.TransactionStatus,
                                        PaymentAmount = payment.PaymentAmount,
                                    };
                                    paymentList.Add(paymentDtoResponse);
                                }
                                else
                                {
                                    throw new KeyNotFoundException("Customer not found");
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                    }
                    return paymentList;
                }
                else
                {
                    return new List<PaymentDTOResponseForAdmin>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaymentDTOResponse> CreateMomoPayment(Order order)
        {
            try
            {
                // Đọc cấu hình MoMo
                var partnerCode = _configuration["MoMo:PartnerCode"];
                var accessKey = _configuration["MoMo:AccessKey"];
                var secretKey = _configuration["MoMo:SecretKey"];
                var returnUrl = _configuration["MoMo:ReturnUrl"];
                var ipnUrl = _configuration["MoMo:IpnUrl"];
                var endpoint = _configuration["MoMo:PaymentEndpoint"];

                // Kiểm tra cấu hình
                if (string.IsNullOrEmpty(partnerCode) || string.IsNullOrEmpty(accessKey) ||
                    string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(returnUrl) ||
                    string.IsNullOrEmpty(ipnUrl) || string.IsNullOrEmpty(endpoint))
                {
                    throw new Exception("Missing MoMo configuration");
                }

                // Tạo thông tin đơn hàng
                var orderInfo = $"Thanh toán đơn hàng {order.OrderId}";
                var amount = (long)order.TotalPrice;
                var orderId = order.OrderId.ToString();
                var requestId = DateTime.UtcNow.Ticks.ToString();

                // Tạo chuỗi để ký
                var rawHash = $"accessKey={accessKey}&" +
                              $"amount={amount}&" +
                              $"extraData=&" +
                              $"ipnUrl={ipnUrl}&" +
                              $"orderId={orderId}&" +
                              $"orderInfo={orderInfo}&" +
                              $"partnerCode={partnerCode}&" +
                              $"redirectUrl={returnUrl}&" +
                              $"requestId={requestId}&" +
                              $"requestType=captureWallet";

                // Tạo chữ ký
                var signature = "";
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
                {
                    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                    signature = BitConverter.ToString(hash).Replace("-", "").ToLower();
                }

                // Tạo payload
                var payload = new
                {
                    partnerCode,
                    requestId,
                    amount,
                    orderId,
                    orderInfo,
                    redirectUrl = returnUrl,
                    ipnUrl,
                    requestType = "captureWallet",
                    extraData = "",
                    lang = "vi",
                    signature
                };

                // Gửi request tới MoMo
                using var client = new HttpClient();
                var response = await client.PostAsJsonAsync(endpoint, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"MoMo API error: {responseContent}");
                }

                // Đọc response
                var responseData = JsonSerializer.Deserialize<JsonDocument>(responseContent);
                var payUrl = responseData.RootElement.GetProperty("payUrl").GetString();

                if (string.IsNullOrEmpty(payUrl))
                {
                    throw new Exception("Invalid MoMo response: missing payUrl");
                }

                return new PaymentDTOResponse
                {
                    PaymentUrl = payUrl
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating MoMo payment: {ex.Message}");
            }
        }

        private string CreateMomoSignature(string message, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public async Task<PaymentDTOResponseForAdmin> GetPaymentById(int paymentId)
        {
            try
            {
                var payments = (await _unitOfWork.PaymentRepository.GetAsync(p => p.PaymentId == paymentId, includeProperties: "Order.Account")).FirstOrDefault();
                if (payments != null)
                {
                    var paymentDtoResponse = new PaymentDTOResponseForAdmin
                    {
                        CustomerName = payments.Order.Account.FullName,
                        PaymentMethod = payments.PaymentMethod,
                        BankCode = payments.BankCode,
                        CardType = payments.CardType,
                        PayDate = payments.PayDate,
                        PaymentAmount = payments.PaymentAmount,
                        OrderId = payments.OrderId,
                        Status = payments.TransactionStatus
                    };
                    return paymentDtoResponse;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public async Task<WalletPaymentResponse> CreateWalletDepositPayment(WalletDepositRequest request)
        {
            try
            {
                // Tạo một transaction history record
                var transaction = new TransactionBalanceHistory
                {
                    CustomerId = request.CustomerId,
                    TransactionType = "Deposit",
                    Amount = request.Amount,
                    TransactionDate = DateTime.Now,
                    Description = "Bạn đã nạp tiền vào ví thành công",
                    BalanceAfterTransaction = 0
                };

                await _unitOfWork.TransactionBalanceHistoryRepository.AddAsync(transaction);
                await _unitOfWork.SaveAsync();

                if (request.PaymentMethod.ToUpper() == "MOMO")
                {
                    // Tạo orderId duy nhất kết hợp giữa prefix, transactionId và timestamp
                    var uniqueOrderId = $"NAP-{transaction.TransactionId}-{DateTime.UtcNow.Ticks}";

                    // Tạo request object với đầy đủ properties
                    var momoRequest = new
                    {
                        partnerCode = _configuration["MoMo:PartnerCode"],
                        requestId = DateTime.UtcNow.Ticks.ToString(),
                        amount = request.Amount,
                        orderId = uniqueOrderId,  // Sử dụng orderId duy nhất
                        orderInfo = $"Nap tien vi - {transaction.TransactionId}",
                        redirectUrl = _configuration["MoMo:ReturnUrl"],
                        ipnUrl = _configuration["MoMo:IpnUrl"],
                        requestType = "captureWallet",
                        extraData = "",
                        lang = "vi",
                        signature = ""
                    };

                    // Tạo chữ ký
                    var rawHash = $"accessKey={_configuration["MoMo:AccessKey"]}&" +
                                 $"amount={momoRequest.amount}&" +
                                 $"extraData={momoRequest.extraData}&" +
                                 $"ipnUrl={momoRequest.ipnUrl}&" +
                                 $"orderId={momoRequest.orderId}&" + // Sử dụng orderId mới
                                 $"orderInfo={momoRequest.orderInfo}&" +
                                 $"partnerCode={momoRequest.partnerCode}&" +
                                 $"redirectUrl={momoRequest.redirectUrl}&" +
                                 $"requestId={momoRequest.requestId}&" +
                                 $"requestType={momoRequest.requestType}";

                    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_configuration["MoMo:SecretKey"])))
                    {
                        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                        var signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                        // Tạo request object mới với signature đã được tính toán
                        momoRequest = new
                        {
                            partnerCode = momoRequest.partnerCode,
                            requestId = momoRequest.requestId,
                            amount = momoRequest.amount,
                            orderId = momoRequest.orderId,
                            orderInfo = momoRequest.orderInfo,
                            redirectUrl = momoRequest.redirectUrl,
                            ipnUrl = momoRequest.ipnUrl,
                            requestType = momoRequest.requestType,
                            extraData = momoRequest.extraData,
                            lang = momoRequest.lang,
                            signature = signature
                        };
                    }

                    // Gọi API MoMo
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            var response = await client.PostAsJsonAsync(_configuration["MoMo:PaymentEndpoint"], momoRequest);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            // Parse response thành dynamic object
                            var responseData = JsonSerializer.Deserialize<JsonDocument>(responseContent);
                            var root = responseData.RootElement;

                            // Kiểm tra và đọc các giá trị từ response
                            if (root.TryGetProperty("resultCode", out var resultCodeElement) &&
                                resultCodeElement.GetInt32() == 0)
                            {
                                string payUrl = root.GetProperty("payUrl").GetString();
                                return new WalletPaymentResponse
                                {
                                    PaymentUrl = payUrl,
                                    Message = "MOMO payment URL created successfully"
                                };
                            }
                            else
                            {
                                // Lấy message lỗi nếu có
                                string errorMessage = root.TryGetProperty("message", out var messageElement)
                                    ? messageElement.GetString()
                                    : "Unknown error from MoMo";

                                throw new Exception($"MoMo payment creation failed: {errorMessage}");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Error processing MoMo payment: {ex.Message}");
                        }
                    }
                }
                else if (request.PaymentMethod.ToUpper() == "VNPAY")
                {
                    var vnpay = new VnPayLibrary();
                    var urlCallBack = _configuration["VNPay:ReturnUrl"];

                    vnpay.AddRequestData("vnp_Version", _configuration["VNPay:Version"]);
                    vnpay.AddRequestData("vnp_Command", "pay");
                    vnpay.AddRequestData("vnp_TmnCode", _configuration["VNPay:TmnCode"]);
                    vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());
                    vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                    vnpay.AddRequestData("vnp_CurrCode", "VND");
                    vnpay.AddRequestData("vnp_IpAddr", "::1");
                    vnpay.AddRequestData("vnp_Locale", "vn");
                    vnpay.AddRequestData("vnp_OrderInfo", $"Nap tien vi - {transaction.TransactionId}");
                    vnpay.AddRequestData("vnp_OrderType", "billpayment");
                    vnpay.AddRequestData("vnp_ReturnUrl", urlCallBack);
                    vnpay.AddRequestData("vnp_TxnRef", transaction.TransactionId.ToString());

                    string paymentUrl = vnpay.CreateRequestUrl(
                        _configuration["VNPay:PaymentUrl"],
                        _configuration["VNPay:HashSecret"]
                    );

                    return new WalletPaymentResponse
                    {
                        PaymentUrl = paymentUrl,
                        Message = "VNPAY payment URL created successfully"
                    };
                }

                throw new Exception("Invalid payment method");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating wallet deposit payment: {ex.Message}");
            }
        }

        public async Task<bool> ProcessWalletDeposit(PaymentDTORequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra trạng thái giao dịch
                    bool isSuccess = false;
                    long transactionId;
                    decimal amount;

                    if (paymentRequest.vnp_ResponseCode != null) // VNPay
                    {
                        isSuccess = paymentRequest.vnp_ResponseCode == "00";
                        transactionId = long.Parse(paymentRequest.vnp_TxnRef);
                        amount = decimal.Parse(paymentRequest.vnp_Amount) / 100;
                    }
                    else // Momo
                    {
                        isSuccess = paymentRequest.resultCode == "0";

                        // Tách orderId để lấy TransactionId
                        var orderIdParts = paymentRequest.orderId.Split('-');
                        transactionId = long.Parse(orderIdParts[1]); // Lấy phần TransactionId

                        amount = decimal.Parse(paymentRequest.amount);
                    }

                    if (!isSuccess) return false;

                    // Lấy transaction history
                    var transactionHistory = await _unitOfWork.TransactionBalanceHistoryRepository
                        .GetByIDAsync(transactionId);

                    if (transactionHistory == null) return false;

                    // Cập nhật hoặc tạo mới ví khách hàng
                    var customerWallet = (await _unitOfWork.CustomerWalletRepository
                        .GetAsync(w => w.CustomerId == transactionHistory.CustomerId))
                        .FirstOrDefault();

                    if (customerWallet == null)
                    {
                        customerWallet = new CustomerWallet
                        {
                            CustomerId = transactionHistory.CustomerId,
                            CustomerBalance = transactionHistory.Amount,
                            UpdateAt = DateTime.Now
                        };
                        await _unitOfWork.CustomerWalletRepository.AddAsync(customerWallet);
                    }
                    else
                    {
                        customerWallet.CustomerBalance += transactionHistory.Amount;
                        customerWallet.UpdateAt = DateTime.Now;
                    }

                    await _unitOfWork.SaveAsync();

                    // Cập nhật số dư sau giao dịch
                    transactionHistory.BalanceAfterTransaction = customerWallet.CustomerBalance;
                    await _unitOfWork.TransactionBalanceHistoryRepository.UpdateAsync(transactionHistory);

                    await _unitOfWork.SaveAsync();

                    // Tạo thông báo sau khi nạp tiền thành công
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(transactionHistory.CustomerId);
                    if (account != null)
                    {
                        await CreateNotification(
                            "Nạp tiền vào ví thành công",
                            $"Bạn đã nạp thành công {amount:N0} VNĐ vào ví. Số dư hiện tại: {customerWallet.CustomerBalance:N0} VNĐ",
                            account.AccountId, "/wallet"
                        );
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProcessWalletDeposit: {ex.Message}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }


        public async Task<bool> ProcessMomoOrderPayment(PaymentDTORequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra trạng thái giao dịch
                    if (paymentRequest.resultCode != "0") return false;

                    long orderId = long.Parse(paymentRequest.orderId);
                    decimal amount = decimal.Parse(paymentRequest.amount);

                    // Lấy thông tin đơn hàng
                    var order = await _unitOfWork.OrderRepository.GetByIDAsync(orderId);
                    if (order == null) return false;

                    // Chuyển đổi timestamp thành DateTime
                    var payDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(paymentRequest.responseTime))
                        .LocalDateTime;

                    // Tạo payment record
                    var payment = new Payment
                    {
                        OrderId = orderId,
                        PaymentMethod = "MoMo",
                        BankCode = "MOMO",
                        BankTransactionNo = paymentRequest.transId,
                        CardType = "QR",
                        PaymentInfo = $"Thanh toán đơn hàng {orderId}",
                        PayDate = payDate,
                        TransactionNo = paymentRequest.transId,
                        TransactionStatus = int.Parse(paymentRequest.resultCode),
                        PaymentAmount = amount
                    };

                    // Cập nhật trạng thái đơn hàng thành đã thanh toán
                    order.Status = 1; // Đã thanh toán
                    await _unitOfWork.OrderRepository.UpdateAsync(order);

                    // Xóa giỏ hàng của người dùng
                    var cartItems = await _unitOfWork.CartItemRepository
                        .GetAsync(c => c.AccountId == order.AccountId && c.Status == true);
                    foreach (var item in cartItems)
                    {
                        await _unitOfWork.CartItemRepository.DeleteAsync(item);
                    }

                    // Lưu payment record
                    await _unitOfWork.PaymentRepository.AddAsync(payment);

                    // Lưu tất cả thay đổi
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    // Tạo thông báo sau khi thanh toán thành công
                    await CreateNotification(
                        "Thanh toán đơn hàng thành công",
                        $"Đơn hàng #{orderId} đã được thanh toán thành công với số tiền {amount:N0} VNĐ qua MoMo.",
                        order.AccountId, $"/order-detail-cus/{order.OrderId}"
                    );

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing MoMo payment: {ex.Message}");
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        private async Task CreateNotification(string title, string description, int accountId, string linkTo)
        {
            // Tạo thông báo
            var notification = new Notification
            {
                Title = title,
                Description = description,
                CreatedDate = DateTime.Now,
                LinkTo = linkTo,
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
        }
    }
}

