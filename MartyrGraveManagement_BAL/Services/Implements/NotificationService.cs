using MartyrGraveManagement_BAL.ModelViews.NotificationDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<NotificationDto> GetNotificationByIdForAccount(int notificationId, int accountId)
        {
            // Kiểm tra nếu NotificationId thuộc về AccountId
            var notificationAccount = await _unitOfWork.NotificationAccountsRepository
                .SingleOrDefaultAsync(na => na.NotificationId == notificationId && na.AccountId == accountId && na.Status == true);

            if (notificationAccount == null)
            {
                return null; // Không tìm thấy hoặc không có quyền
            }

            // Lấy thông tin chi tiết của thông báo
            var notification = await _unitOfWork.NotificationRepository.GetByIDAsync(notificationId);

            if (notification == null)
            {
                return null;
            }
            notificationAccount.isRead = true;
            await _unitOfWork.NotificationAccountsRepository.UpdateAsync(notificationAccount);
            await _unitOfWork.SaveAsync();
            // Trả về dưới dạng DTO
            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                Title = notification.Title,
                Description = notification.Description,
                CreatedDate = notification.CreatedDate,
                isRead = notificationAccount.isRead,
                Status = notification.Status
            };
        }

        public async Task<bool> LinkNotificationToAllCustomerAccounts(int notificationId)
        {
            try
            {
                // Lấy danh sách tất cả các tài khoản khách hàng (RoleId là 4)
                var customerAccounts = await _unitOfWork.AccountRepository.GetAsync(a => a.RoleId == 4 && a.Status == true);

                // Tạo danh sách NotificationAccount với trạng thái là false
                var notificationAccounts = customerAccounts.Select(account => new NotificationAccount
                {
                    AccountId = account.AccountId,
                    NotificationId = notificationId,
                    Status = false // Đặt trạng thái là false
                }).ToList();

                await _unitOfWork.NotificationAccountsRepository.AddRangeAsync(notificationAccounts);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error linking notification to all customer accounts: {ex.Message}");
                return false;
            }
        }

        public async Task<NotificationResponseDto> GetNotificationByIdAsync(int notificationId)
        {
            // Lấy thông tin Notification
            var notification = await _unitOfWork.NotificationRepository.GetByIDAsync(notificationId);
            if (notification == null)
            {
                return null;
            }

            // Lấy danh sách NotificationAccount cùng với thông tin Account liên kết
            var notificationAccounts = await _unitOfWork.NotificationAccountsRepository.GetAsync(
                na => na.NotificationId == notificationId,
                includeProperties: "Account" // Bao gồm bảng Account trong truy vấn
            );

            // Tạo DTO và ánh xạ dữ liệu
            return new NotificationResponseDto
            {
                NotificationId = notification.NotificationId,
                Title = notification.Title,
                Description = notification.Description,
                CreatedDate = notification.CreatedDate,
                Status = notification.Status,
                NotificationAccounts = notificationAccounts.Select(na => new NotificationAccountDto
                {
                    AccountId = na.AccountId,
                    Status = na.Status,
                    FullName = na.Account?.FullName,       
                    AvatarPath = na.Account?.AvatarPath    
                }).ToList()
            };
        }





        public async Task<List<Notification>> GetAllNotifications()
        {
            var notifications = await _unitOfWork.NotificationRepository.GetAllAsync();
            return notifications.ToList();
        }

        public async Task<(List<NotificationDto> notifications, int totalPage)> GetNotificationsByAccountId(
            int accountId, 
            int pageIndex, 
            int pageSize)
        {
            try
            {
                int totalPage = 0;
                int totalNotifications = 0;

                // Tạo filter expression
                Expression<Func<NotificationAccount, bool>> filter = 
                    na => na.AccountId == accountId && na.Status == true;

                // Đếm tổng số thông báo
                totalNotifications = (await _unitOfWork.NotificationAccountsRepository.GetAsync(filter)).Count();
                totalPage = (int)Math.Ceiling(totalNotifications / (double)pageSize);

                // Lấy danh sách thông báo với phân trang
                var notificationAccounts = await _unitOfWork.NotificationAccountsRepository.GetAsync(
                    filter: filter,
                    includeProperties: "Notification,Account",
                    orderBy: q => q.OrderByDescending(na => na.Notification.CreatedDate),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );

                if (!notificationAccounts.Any())
                {
                    return (new List<NotificationDto>(), 0);
                }

                var notificationDtos = notificationAccounts.Select(na => new NotificationDto
                {
                    NotificationId = na.Notification.NotificationId,
                    Title = na.Notification.Title,
                    Description = na.Notification.Description,
                    CreatedDate = na.Notification.CreatedDate,
                    isRead = na.isRead,
                    Status = na.Notification.Status,
                    NotificationAccounts = new List<NotificationAccountDto>
                    {
                        new NotificationAccountDto
                        {
                            AccountId = na.AccountId,
                            Status = na.Status,
                            FullName = na.Account?.FullName,
                            AvatarPath = na.Account?.AvatarPath
                        }
                    }
                }).ToList();

                return (notificationDtos, totalPage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




        public async Task<bool> UpdateNotificationAccountStatus(int notificationId, bool newStatus)
        {
            try
            {
                // Lấy danh sách các NotificationAccount liên kết với notificationId
                var notificationAccounts = await _unitOfWork.NotificationAccountsRepository
                    .GetAsync(na => na.NotificationId == notificationId);

                if (notificationAccounts == null || !notificationAccounts.Any())
                {
                    return false; // Không tìm thấy NotificationAccount
                }

                // Cập nhật trạng thái cho từng NotificationAccount
                foreach (var notificationAccount in notificationAccounts)
                {
                    notificationAccount.Status = newStatus;
                    await _unitOfWork.NotificationAccountsRepository.UpdateAsync(notificationAccount);
                }

                await _unitOfWork.SaveAsync(); // Lưu thay đổi sau khi cập nhật tất cả các NotificationAccount

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating notification account status: {ex.Message}");
                return false;
            }
        }



    }
}
