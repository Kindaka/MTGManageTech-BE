﻿using MartyrGraveManagement.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement.BackgroundServices.Implements
{
    public class TaskBackgroundService : ITaskBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckExpiredTasks()
        {

            // Lấy các task có trạng thái "đang thực hiện" (status 0123) và hết hạn
            var expiredTasks = await _unitOfWork.TaskRepository.GetAsync(task =>
                       task.Status < 4 && task.Status >= 0 && task.EndDate <= DateTime.Now);

            if (expiredTasks.Any())
            {
                foreach (var task in expiredTasks)
                {
                    // Cập nhật trạng thái task thành thất bại (status 5)
                    task.Status = 5;

                    // Lấy Order liên quan đến task và cập nhật trạng thái Order thành thất bại
                    var order = await _unitOfWork.OrderRepository.GetByIDAsync(task.OrderId);
                    if (order != null)
                    {
                        order.Status = 5; // Cập nhật trạng thái Order thành thất bại
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

                    // Cập nhật task
                    await _unitOfWork.TaskRepository.UpdateAsync(task);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveAsync();
            }


        }
    }
}
