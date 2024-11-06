using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.BackgroundServices.Implements
{
    public class AttendanceBackgroundService : IAttendanceBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task MarkAbsentAttendanceAsync()
        {
            while (true)
            {
                try
                {
                    // Lấy các bản ghi Attendance với ngày nhỏ hơn ngày hiện tại và Status = 0
                    var pendingAttendances = await _unitOfWork.AttendanceRepository
                        .GetAsync(a => a.Date < DateOnly.FromDateTime(DateTime.Now) && a.Status == 0);

                    if (pendingAttendances.Any())
                    {
                        foreach (var attendance in pendingAttendances)
                        {
                            // Cập nhật trạng thái thành Absent (2)
                            attendance.Status = 2;
                            attendance.UpdatedAt = DateTime.Now;

                            // Cập nhật attendance
                            await _unitOfWork.AttendanceRepository.UpdateAsync(attendance);
                        }

                        // Lưu thay đổi vào cơ sở dữ liệu
                        await _unitOfWork.SaveAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi
                    Console.WriteLine($"Error occurred in MarkAbsentAttendanceAsync: {ex.Message}");
                }

                // Đặt khoảng thời gian kiểm tra (ví dụ 10 giây cho mục đích kiểm tra)
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

    }
}
