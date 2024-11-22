using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
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
                        ? int.Parse(paymentRequest.vnp_TxnRef) 
                        : int.Parse(paymentRequest.orderId));

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
                        OrderId = int.Parse(paymentRequest.vnp_TxnRef)
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
                        OrderId = int.Parse(paymentRequest.orderId)
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
                }

                await _unitOfWork.SaveAsync();
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

                if( payments != null && payments.Any())
                {
                    var paymentList = new List<PaymentDTOResponseForAdmin>();
                    foreach ( var payment in payments )
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
                // Tạo payload MoMo
                var partnerCode = _configuration["MoMo:PartnerCode"];
                var accessKey = _configuration["MoMo:AccessKey"];
                var secretKey = _configuration["MoMo:SecretKey"];
                var returnUrl = _configuration["MoMo:ReturnUrl"];
                var ipnUrl = _configuration["MoMo:IpnUrl"];
                var endpoint = _configuration["MoMo:PaymentEndpoint"];
                var orderInfo = $"Thanh toán đơn hàng {order.OrderId}";
                var amount = (long)order.TotalPrice;
                var orderId = order.OrderId.ToString();
                var requestId = Guid.NewGuid().ToString();

                // Tạo chữ ký
                var rawHash = $"accessKey={accessKey}&amount={amount}&extraData=&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType=captureWallet";
                var signature = CreateMomoSignature(rawHash, secretKey);

                var payload = new
                {
                    partnerCode,
                    accessKey,
                    requestId,
                    orderId,
                    orderInfo,
                    redirectUrl = returnUrl,
                    ipnUrl,
                    amount,
                    requestType = "captureWallet",
                    extraData = "",
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

                var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
                
                // Chỉ tạo bản ghi Payment khi nhận được callback thành công
                // KHÔNG tạo Payment record tại đây
                
                return new PaymentDTOResponse 
                { 
                    PaymentUrl = responseData.payUrl.ToString() // Trả về URL thanh toán từ MoMo
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
            catch (Exception ex) {
                throw new Exception($"Error: {ex.Message}");
            }
        }
    }
}

