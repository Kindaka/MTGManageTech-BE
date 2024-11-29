using MartyrGraveManagement_BAL.ModelViews.NotificationDTOs;
using MartyrGraveManagement_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsByAccountId(int accountId);
        Task<List<Notification>> GetAllNotifications();
        Task<bool> LinkNotificationToAllCustomerAccounts(int notificationId);

        Task<NotificationResponseDto> GetNotificationByIdAsync(int notificationId);
        Task<bool> UpdateNotificationAccountStatus(int notificationId, bool newStatus);
        Task<NotificationDto> GetNotificationByIdForAccount(int notificationId, int accountId);
    }
}
