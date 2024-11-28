using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.AssignmentTaskDtoRequest;
using MartyrGraveManagement_BAL.ModelViews.ServiceScheduleDTOs;
using MartyrGraveManagement_BAL.ModelViews.TaskDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Implements
{
    public class AssignmentTaskService : IAssignmentTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AssignmentTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateTasksAsync(int serviceScheduleId)
        {
            try
            {
                var taskResponses = new List<TaskDtoResponse>();


                    // Kiểm tra xem OrderId có tồn tại không
                    var serviceSchedule = (await _unitOfWork.ServiceScheduleRepository.GetAsync(s => s.ServiceScheduleId == serviceScheduleId, includeProperties:"Service")).FirstOrDefault();
                    if (serviceSchedule == null)
                    {
                        throw new KeyNotFoundException("Id không tồn tại.");
                    }

                    // Kiểm tra trạng thái của Order, nếu không phải là 1 thì không cho tạo Task
                    if (serviceSchedule.Status != true)
                    {
                        throw new InvalidOperationException("Đơn hàng không ở trạng thái hợp lệ để tạo Task.");
                    }


                    // Kiểm tra xem khu vực của MartyrGrave trong OrderDetail
                    var martyrGrave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(serviceSchedule.MartyrId);
                    if (martyrGrave == null)
                    {
                        throw new InvalidOperationException("MartyrGrave không tồn tại.");
                    }

                    // Lấy danh sách nhân viên thuộc cùng khu vực
                    var staffAccounts = await _unitOfWork.AccountRepository
                        .FindAsync(a => a.RoleId == 3 && a.AreaId == martyrGrave.AreaId);

                    if (!staffAccounts.Any())
                    {
                        throw new InvalidOperationException("Không có nhân viên nào thuộc khu vực phù hợp để nhận công việc.");
                    }

                    // Sắp xếp danh sách nhân viên dựa trên số lượng công việc
                    var staffWorkloads = new Dictionary<int, int>();
                    foreach (var staff in staffAccounts)
                    {
                        var taskCount = await _unitOfWork.TaskRepository
                            .CountAsync(t => t.AccountId == staff.AccountId); // Lấy công việc đang được chỉ định
                        staffWorkloads[staff.AccountId] = taskCount;
                    }

                    var sortedStaffAccounts = staffAccounts
                        .OrderBy(staff => staffWorkloads[staff.AccountId])
                        .ToList();

                    // Lấy nhân viên có ít công việc nhất
                    var selectedStaff = sortedStaffAccounts.First();

                    DateTime? serviceDate = null;

                    if (serviceSchedule.Service.RecurringType == 1)
                    {
                        serviceDate = GetNextWeeklyServiceDate(serviceSchedule.DayOfWeek);
                    }
                    else if (serviceSchedule.Service.RecurringType == 2)
                    {
                        serviceDate = GetNextMonthlyServiceDate(serviceSchedule.DayOfMonth);
                    }

                    // Tạo task mới và gắn Note của Order vào Description
                    var taskEntity = new AssignmentTask
                    {
                        StaffId = selectedStaff.AccountId,
                        ServiceScheduleId = serviceScheduleId,
                        CreateAt = DateTime.Now,
                        EndDate = (DateTime)serviceDate,
                        Description = serviceSchedule.Note,  // Gắn Note của Order vào Description của Task
                        Status = 1  // Trạng thái ban đầu là 'assigned'
                    };

                    // Thêm Task vào cơ sở dữ liệu
                    await _unitOfWork.AssignmentTaskRepository.AddAsync(taskEntity);

                    
                

                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
