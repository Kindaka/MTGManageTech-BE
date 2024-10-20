﻿using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.PaymentDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        //public async Task<PaymentDTOResponse> CreatePayment(PaymentDTORequest paymentRequest)
        //{
        //    using (var transaction = await _unitOfWork.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            var existedOrder = await _unitOfWork.OrderRepository.GetByIDAsync(int.Parse(paymentRequest.vnp_TxnRef));
        //            if (existedOrder != null)
        //            {
        //                var existedPayment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == existedOrder.OrderId);
        //                if (existedPayment.Any())
        //                {
        //                    return null;
        //                }
        //                var payment = new Payment()
        //                {
        //                    PaymentMethod = "VNPay",
        //                    BankCode = paymentRequest.vnp_BankCode,

        //                    BankTransactionNo = paymentRequest.vnp_BankTranNo,

        //                    CardType = paymentRequest.vnp_CardType,
        //                    PaymentInfo = paymentRequest.vnp_OrderInfo,
        //                    PayDate = DateTime.ParseExact(paymentRequest.vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        //                    TransactionNo = paymentRequest.vnp_TransactionNo,
        //                    TransactionStatus = int.Parse(paymentRequest.vnp_TransactionStatus),
        //                    PaymentAmount = decimal.Parse(paymentRequest.vnp_Amount) / 100,
        //                    OrderId = int.Parse(paymentRequest.vnp_TxnRef)
        //                };
        //                await _unitOfWork.PaymentRepository.AddAsync(payment);

        //                // Update Order's status is Paid
        //                existedOrder.Status = 1;
        //                await _unitOfWork.OrderRepository.UpdateAsync(existedOrder);

        //                // accumulate points customer point
        //                /*var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(existedOrder.CustomerId);
        //                customer.Point += (int)((double)payment.PaymentAmount * 0.02); // 2% per successful order
        //                await _unitOfWork.CustomerRepository.UpdateAsync(customer);*/

        //                await _unitOfWork.SaveAsync();
        //                await transaction.CommitAsync();
        //                return _mapper.Map<PaymentDTOResponse>(payment);
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            throw new Exception(ex.Message);
        //        }
        //    }
        //}


        public async Task<PaymentDTOResponse> CreatePayment(PaymentDTORequest paymentRequest)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Lấy thông tin đơn hàng dựa vào mã giao dịch thanh toán
                    var existedOrder = await _unitOfWork.OrderRepository.GetByIDAsync(int.Parse(paymentRequest.vnp_TxnRef));
                    if (existedOrder != null)
                    {
                        // Kiểm tra nếu đã có thanh toán cho đơn hàng này
                        var existedPayment = await _unitOfWork.PaymentRepository.GetAsync(p => p.OrderId == existedOrder.OrderId);
                        if (existedPayment.Any())
                        {
                            return null;
                        }

                        // Tạo thông tin thanh toán mới
                        var payment = new Payment
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

                        // Nếu giao dịch thành công
                        if (payment.TransactionStatus == 00)
                        {
                            // Cập nhật trạng thái đơn hàng là Đã thanh toán (Status = 1)
                            existedOrder.Status = 1;
                            await _unitOfWork.OrderRepository.UpdateAsync(existedOrder);

                            // Xóa giỏ hàng sau khi thanh toán thành công
                            var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == existedOrder.AccountId && c.Status == true);
                            foreach (var cartItem in cartItems)
                            {
                                if (cartItem.Status == true)
                                {
                                    await _unitOfWork.CartItemRepository.DeleteAsync(cartItem);
                                }
                            }

                            await _unitOfWork.SaveAsync();
                            await transaction.CommitAsync();
                        }
                        else
                        {
                            // Nếu thanh toán không thành công, không xóa giỏ hàng và không cập nhật trạng thái đơn hàng
                            await transaction.RollbackAsync();
                            throw new Exception("Payment failed, transaction was not successful.");
                        }

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



        public async Task<List<PaymentDTOResponseForAdmin>> GetPaymentList(DateTime startDate, DateTime endDate, int? status)
        {
            try
            {
                var payments = await _unitOfWork.PaymentRepository.GetAsync(p => p.PayDate >= startDate.Date && p.PayDate <= endDate.Date.AddDays(1).AddTicks(-1));

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
                                        CustomerName = customer.FullName,
                                        PayDate = payment.PayDate,
                                        Status = order.Status,
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
                                        CustomerName = customer.FullName,
                                        PayDate = payment.PayDate,
                                        Status = order.Status,
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
    }
}

