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
    public class RecurringTaskService : IRecurringTaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecurringTaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateRecurringTasksAsync()
        {
            while (true)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var activeServiceSchedules = await _unitOfWork.ServiceScheduleRepository
                .GetAsync(ss => ss.Status == true, includeProperties: "Service,Account");

                        foreach (var serviceSchedule in activeServiceSchedules)
                        {
                            DateTime? nextServiceDate = null;

                            // Tính ngày dịch vụ tiếp theo
                            if (serviceSchedule.Service.RecurringType == 1) // Hàng tuần
                            {
                                nextServiceDate = GetNextWeeklyServiceDate(serviceSchedule.DayOfWeek);
                            }
                            else if (serviceSchedule.Service.RecurringType == 2) // Hàng tháng
                            {
                                nextServiceDate = GetNextMonthlyServiceDate(serviceSchedule.DayOfMonth);
                            }

                            // Kiểm tra nếu `nextServiceDate` hợp lệ và tạo công việc trước 7 ngày
                            if (nextServiceDate.HasValue)
                            {
                                if (DateTime.Today >= nextServiceDate.Value.AddDays(-6) && DateTime.Today < nextServiceDate.Value)
                                {
                                    // Kiểm tra xem công việc đã tồn tại chưa
                                    //var existingTask = await _unitOfWork.AssignmentTaskRepository
                                    //    .FindAsync(t => t.ServiceScheduleId == serviceSchedule.ServiceScheduleId && t.EndDate == nextServiceDate.Value);

                                    var existingTask = await _unitOfWork.AssignmentTaskRepository
                                    .FindAsync(t => t.ServiceScheduleId == serviceSchedule.ServiceScheduleId && DateOnly.FromDateTime(t.EndDate) == DateOnly.FromDateTime(nextServiceDate.Value));

                                    if (existingTask.Any())
                                    {
                                        Console.WriteLine($"Công việc đã tồn tại cho ServiceSchedule ID {serviceSchedule.ServiceScheduleId} vào ngày {nextServiceDate.Value}");
                                        continue;
                                    }

                                    var customer = (await _unitOfWork.CustomerWalletRepository.GetAsync(c => c.CustomerId == serviceSchedule.Account.AccountId)).FirstOrDefault();
                                    if (customer == null)
                                    {
                                        // Ghi log hoặc thông báo nếu không đủ số dư
                                        Console.WriteLine($"Khách hàng {customer.CustomerId} không tìm thấy ví.");
                                        continue;
                                    }

                                    // Kiểm tra số dư ví của khách hàng
                                    if (customer.CustomerBalance < serviceSchedule.Service.Price)
                                    {
                                        // Ghi log hoặc thông báo nếu không đủ số dư
                                        Console.WriteLine($"Khách hàng {customer.CustomerId} không đủ số dư để tạo công việc.");
                                        continue;
                                    }

                                    // Trừ số dư ví của khách hàng
                                    customer.CustomerBalance -= serviceSchedule.Service.Price;

                                    // Lưu lịch sử giao dịch
                                    var transactionHistory = new TransactionBalanceHistory
                                    {
                                        CustomerId = customer.CustomerId,
                                        Amount = -(serviceSchedule.Service.Price),
                                        TransactionDate = DateTime.Now,
                                        Description = $"Thanh toán dịch vụ định kì {serviceSchedule.Service.ServiceName} (Lịch ID: {serviceSchedule.ServiceScheduleId})",
                                        TransactionType = "Payment" // Loại giao dịch: Trừ tiền
                                    };

                                    await _unitOfWork.TransactionBalanceHistoryRepository.AddAsync(transactionHistory);

                                    // Lấy thông tin MartyrGrave liên quan
                                    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(serviceSchedule.MartyrId);
                                    if (martyrGrave == null) continue;

                                    // Lấy danh sách nhân viên phù hợp
                                    var staffAccounts = await _unitOfWork.AccountRepository
                                        .FindAsync(a => a.RoleId == 3 && a.AreaId == martyrGrave.AreaId);

                                    if (!staffAccounts.Any()) continue;

                                    // Sắp xếp nhân viên theo số lượng công việc hiện tại
                                    var staffWorkloads = new Dictionary<int, int>();
                                    foreach (var staff in staffAccounts)
                                    {
                                        var taskCount = await _unitOfWork.TaskRepository
                                            .CountAsync(t => t.AccountId == staff.AccountId);
                                        staffWorkloads[staff.AccountId] = taskCount;
                                    }

                                    var selectedStaff = staffAccounts
                                        .OrderBy(staff => staffWorkloads[staff.AccountId])
                                        .First();

                                    // Tạo công việc mới với ngày kết thúc là `nextServiceDate`
                                    var taskEntity = new AssignmentTask
                                    {
                                        StaffId = selectedStaff.AccountId,
                                        ServiceScheduleId = serviceSchedule.ServiceScheduleId,
                                        CreateAt = DateTime.Now,
                                        EndDate = nextServiceDate.Value, // Ngày hoàn thành công việc
                                        Description = serviceSchedule.Note,
                                        Status = 1 // Trạng thái ban đầu
                                    };

                                    await _unitOfWork.AssignmentTaskRepository.AddAsync(taskEntity);
                                }
                            }
                        }

                        await transaction.CommitAsync();
                    }catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        // Log error
                        Console.WriteLine($"Error in CheckServiceSchedule: {ex.Message}");
                    }
                }
                await Task.Delay(10000);
            }
        }

        public DateTime GetNextWeeklyServiceDate(int dayOfService)
        {
            var today = DateTime.Now;
            int currentDayOfWeek = (int)today.DayOfWeek; // 0 = Chủ Nhật, ..., 6 = Thứ Bảy

            // Điều chỉnh để phù hợp với DayOfService (1 = Chủ Nhật, ..., 7 = Thứ Bảy)
            currentDayOfWeek = currentDayOfWeek == 0 ? 7 : currentDayOfWeek; // Chủ Nhật => 7

            int daysUntilNextService = (dayOfService - currentDayOfWeek + 7) % 7;
            return today.AddDays(daysUntilNextService == 0 ? 7 : daysUntilNextService); // Nếu trùng ngày, lấy tuần sau
        }

        public DateTime? GetNextMonthlyServiceDate(int dayOfService)
        {
            var today = DateTime.Now;

            // Lấy ngày trong tháng hiện tại
            int currentDay = today.Day;

            // Nếu ngày hiện tại < DayOfService, dịch vụ sẽ diễn ra trong tháng này
            if (currentDay < dayOfService)
            {
                return new DateTime(today.Year, today.Month, dayOfService);
            }

            // Nếu không, chuyển sang tháng tiếp theo
            int nextMonth = today.Month + 1;
            int year = today.Year;

            if (nextMonth > 12)
            {
                nextMonth = 1;
                year++;
            }

            // Kiểm tra nếu tháng tiếp theo có đủ số ngày
            int daysInNextMonth = DateTime.DaysInMonth(year, nextMonth);
            if (dayOfService > daysInNextMonth)
            {
                return null; // Không thể lập lịch do ngày vượt quá số ngày trong tháng
            }

            return new DateTime(year, nextMonth, dayOfService);
        }
    }
}
