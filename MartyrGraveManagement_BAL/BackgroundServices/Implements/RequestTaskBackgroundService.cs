using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement_BAL.BackgroundServices.Implements
{
    public class RequestTaskBackgroundService : IRequestTaskBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestTaskBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckExpiredRequestTask()
        {
            // Lấy các task có trạng thái "đang thực hiện" (status 0123) và hết hạn
            var expiredRequestTasks = await _unitOfWork.RequestTaskRepository.GetAsync(task =>
                       task.Status < 4 && task.Status >= 0 && task.EndDate <= DateOnly.FromDateTime(DateTime.Now));

            if (expiredRequestTasks.Any())
            {
                foreach (var task in expiredRequestTasks)
                {
                    // Cập nhật trạng thái task thành thất bại (status 5)
                    task.Status = 5;

                    // Lấy Order liên quan đến task và cập nhật trạng thái Order thành thất bại
                    var request = await _unitOfWork.RequestCustomerRepository.GetByIDAsync(task.RequestId);
                    if (request != null)
                    {
                        request.Status = 8; // Cập nhật trạng thái Request thành thất bại
                        await _unitOfWork.RequestCustomerRepository.UpdateAsync(request);
                        // Kiểm tra và hoàn tiền vào ví khách hàng
                        var customerWallet = (await _unitOfWork.CustomerWalletRepository
                            .GetAsync(w => w.CustomerId == request.CustomerId))
                            .FirstOrDefault();

                        if (customerWallet != null)
                        {
                            // Cập nhật số dư ví
                            customerWallet.CustomerBalance += request.Price;
                            customerWallet.UpdateAt = DateTime.Now;
                            await _unitOfWork.CustomerWalletRepository.UpdateAsync(customerWallet);

                            // Tạo lịch sử giao dịch
                            var transactionHistory = new TransactionBalanceHistory
                            {
                                CustomerId = request.CustomerId,
                                Amount = request.Price,
                                TransactionType = "Refund",
                                Description = $"Hoàn tiền cho yêu cầu dịch vụ riêng #{request.RequestId} vì đã hết hạn",
                                TransactionDate = DateTime.Now,
                                BalanceAfterTransaction = customerWallet.CustomerBalance
                            };
                            await _unitOfWork.TransactionBalanceHistoryRepository.AddAsync(transactionHistory);

                            // Tạo thông báo cho khách hàng
                            var notification = new Notification
                            {
                                Title = "Hoàn tiền dịch vụ yêu cầu.",
                                Description = $"Yêu cầu #{request.RequestId} đã hết hạn. Số tiền {request.Price:N0} VNĐ đã được hoàn vào ví của bạn.",
                                CreatedDate = DateTime.Now,
                                Status = true
                            };
                            await _unitOfWork.NotificationRepository.AddAsync(notification);

                            // Liên kết thông báo với tài khoản
                            var notificationAccount = new NotificationAccount
                            {
                                AccountId = request.CustomerId,
                                NotificationId = notification.NotificationId,
                                Status = true
                            };
                            await _unitOfWork.NotificationAccountsRepository.AddAsync(notificationAccount);
                        }
                    }

                    // Cập nhật task
                    await _unitOfWork.RequestTaskRepository.UpdateAsync(task);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
