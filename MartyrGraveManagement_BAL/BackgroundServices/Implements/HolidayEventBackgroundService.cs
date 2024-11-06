using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
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

        //public async Task MarkNotificationsForUpcomingHolidays()
        //{
        //    while (true)
        //    {
        //        var today = DateOnly.FromDateTime(DateTime.Today);
        //        var upcomingEvents = await _unitOfWork.HolidayEventsRepository.GetAsync(
        //            e => e.EventDate >= today && e.EventDate <= today.AddDays(2) && e.Status == true);

        //        if (upcomingEvents.Any())
        //        {
        //            foreach (var holidayEvent in upcomingEvents)
        //            {
        //                // Lấy thông báo liên quan dựa trên ID của sự kiện (giả sử có thông báo cho mỗi sự kiện)
        //                var notification = await _unitOfWork.NotificationRepository
        //                    .SingleOrDefaultAsync(n => n.Title.Contains(holidayEvent.EventName));

        //                if (notification != null)
        //                {
        //                    // Cập nhật trạng thái của NotificationAccount thành true cho tất cả tài khoản khách hàng liên kết
        //                    var notificationAccounts = await _unitOfWork.NotificationAccountsRepository
        //                        .GetAsync(na => na.NotificationId == notification.NotificationId && na.Status == false);

        //                    foreach (var notificationAccount in notificationAccounts)
        //                    {
        //                        notificationAccount.Status = true; // Đánh dấu là hiển thị cho khách hàng
        //                        await _unitOfWork.NotificationAccountsRepository.UpdateAsync(notificationAccount);
        //                    }

        //                    // Lưu thay đổi
        //                    await _unitOfWork.SaveAsync();
        //                }
        //            }
        //        }

        //        // Thêm delay 10 giây giữa mỗi lần kiểm tra
        //        await Task.Delay(10000);
        //    }
        //}

        public async Task UpdateNotificationAccountsForUpcomingDay()
        {
            while (true)
            {
                try
                {
                    var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

                    // Lấy tất cả các thông báo có ngày tạo bằng ngày mai
                    var notificationsUpcoming = await _unitOfWork.NotificationRepository
                        .GetAsync(n => DateOnly.FromDateTime(n.CreatedDate.Date) == tomorrow);

                    if (notificationsUpcoming.Any())
                    {
                        foreach (var notification in notificationsUpcoming)
                        {
                            // Lấy tất cả các NotificationAccount liên quan đến Notification này có status là false
                            var notificationAccounts = await _unitOfWork.NotificationAccountsRepository
                                .GetAsync(na => na.NotificationId == notification.NotificationId && na.Status == false);

                            // Cập nhật status thành true cho tất cả các NotificationAccount liên quan
                            foreach (var notificationAccount in notificationAccounts)
                            {
                                notificationAccount.Status = true;
                                await _unitOfWork.NotificationAccountsRepository.UpdateAsync(notificationAccount);
                            }

                            // Lưu thay đổi
                            await _unitOfWork.SaveAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi
                    Console.WriteLine($"Error in UpdateNotificationAccountsForUpcomingDay: {ex.Message}");
                }

                // Chờ 10 giây trước khi lặp lại lần tiếp theo
                await Task.Delay(10000);
            }
        }





    }

}
