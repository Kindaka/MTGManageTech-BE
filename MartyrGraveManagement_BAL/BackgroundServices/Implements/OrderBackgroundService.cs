using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.BackgroundServices.Implements
{
    public class OrderBackgroundService : IOrderBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckExpiredOrderPayment()
        {
            while (true)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var expiredOrders = await _unitOfWork.OrderRepository.GetAsync(order =>
                            order.Status == 0 && order.OrderDate.AddMinutes(10) <= DateTime.Now);

                        if (expiredOrders.Any())
                        {
                            foreach (var order in expiredOrders)
                            {
                                // Cập nhật trạng thái đơn hàng thành đã hủy (status 2)
                                order.Status = 2;
                                await _unitOfWork.OrderRepository.UpdateAsync(order);

                                // Kiểm tra và hoàn tiền vào ví khách hàng
                                var customerWallet = (await _unitOfWork.CustomerWalletRepository
                                    .GetAsync(w => w.CustomerId == order.AccountId))
                                    .FirstOrDefault();

                                if (customerWallet != null)
                                {
                                    // Cập nhật số dư ví
                                    customerWallet.CustomerBalance += order.TotalPrice;
                                    customerWallet.UpdateAt = DateTime.Now;
                                    await _unitOfWork.CustomerWalletRepository.UpdateAsync(customerWallet);

                                    // Tạo lịch sử giao dịch
                                    var transactionHistory = new TransactionBalanceHistory
                                    {
                                        CustomerId = order.AccountId,
                                        Amount = order.TotalPrice,
                                        TransactionType = "Refund",
                                        Description = $"Hoàn tiền cho đơn hàng #{order.OrderId} đã hết hạn",
                                        TransactionDate = DateTime.Now,
                                        BalanceAfterTransaction = customerWallet.CustomerBalance
                                    };
                                    await _unitOfWork.TransactionBalanceHistoryRepository.AddAsync(transactionHistory);

                                    // Tạo thông báo cho khách hàng
                                    var notification = new Notification
                                    {
                                        Title = "Hoàn tiền đơn hàng",
                                        Description = $"Đơn hàng #{order.OrderId} đã hết hạn thanh toán. Số tiền {order.TotalPrice:N0} VNĐ đã được hoàn vào ví của bạn.",
                                        CreatedDate = DateTime.Now,
                                        Status = true
                                    };
                                    await _unitOfWork.NotificationRepository.AddAsync(notification);

                                    // Liên kết thông báo với tài khoản
                                    var notificationAccount = new NotificationAccount
                                    {
                                        AccountId = order.AccountId,
                                        NotificationId = notification.NotificationId,
                                        Status = true
                                    };
                                    await _unitOfWork.NotificationAccountsRepository.AddAsync(notificationAccount);
                                }
                            }

                            // Lưu tất cả thay đổi
                            await _unitOfWork.SaveAsync();
                            await transaction.CommitAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        // Log error
                        Console.WriteLine($"Error in CheckExpiredOrderPayment: {ex.Message}");
                    }
                }
                await Task.Delay(10000);
            }
        }
    }
}
