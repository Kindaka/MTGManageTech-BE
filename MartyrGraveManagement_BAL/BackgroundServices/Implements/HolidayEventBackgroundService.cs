using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace MartyrGraveManagement_BAL.BackgroundServices.Implements
{
    public class HolidayEventBackgroundService : IHolidayEventBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HolidayEventBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckAndSendNotificationsForUpcomingHolidayEvents()
        {
            while (true)
            {
                // Lấy các sự kiện diễn ra trong ngày tiếp theo
                var upcomingEvents = await _unitOfWork.HolidayEventsRepository.GetAsync(e =>
                    e.EventDate == DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7).AddDays(1)) && e.Status == true);

                if (upcomingEvents.Any())
                {
                    foreach (var holidayEvent in upcomingEvents)
                    {
                        // Tạo thông báo mới cho sự kiện
                        var notification = new Notification
                        {
                            Title = $"Thông báo sự kiện {holidayEvent.EventName}",
                            Description = $"Sự kiện {holidayEvent.EventName} sẽ diễn ra vào ngày {holidayEvent.EventDate}.",
                            CreatedDate = DateTime.UtcNow.AddHours(7),
                            Status = true
                        };
                        await _unitOfWork.NotificationRepository.AddAsync(notification);
                        await _unitOfWork.SaveAsync();

                        // Lấy danh sách tất cả các khách hàng (RoleId là khách hàng)
                        var customers = await _unitOfWork.AccountRepository.GetAsync(a => a.RoleId == 4 && a.Status == true);

                        // Chia nhỏ thông báo để lưu theo lô
                        var notificationAccounts = customers.Select(c => new NotificationAccount
                        {
                            AccountId = c.AccountId,
                            NotificationId = notification.NotificationId,
                            Status = true // Đánh dấu là thông báo mới
                        }).ToList();

                        int batchSize = 100; // Số lượng bản ghi mỗi lô
                        for (int i = 0; i < notificationAccounts.Count; i += batchSize)
                        {
                            var batch = notificationAccounts.Skip(i).Take(batchSize).ToList();
                            await _unitOfWork.NotificationAccountsRepository.AddRangeAsync(batch);
                            await _unitOfWork.SaveAsync();
                        }
                    }
                }

                // Đợi 10 giây trước khi kiểm tra lại (dùng cho mục đích test)
                await Task.Delay(10000);
            }
        }


    }

}
