using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.ScheduleDTOs;
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
    public class ScheduleService : IScheduleService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<List<string>> CreateSchedule(List<CreateScheduleDTORequest> requests, int accountId)
        {
            try
            {
                var results = new List<string>();

                foreach (var request in requests)
                {
                    //// 1. Kiểm tra xem tài khoản có phải là nhân viên không
                    //var account = await _unitOfWork.AccountRepository.GetByIDAsync(request.AccountId);
                    //if (account == null || account.RoleId != 3)
                    //{
                    //    results.Add($"Tài khoản ID {request.AccountId} không phải là nhân viên.");
                    //    continue;
                    //}

                    //// 2. Lấy thông tin Task
                    //var task = await _unitOfWork.TaskRepository.GetByIDAsync(request.TaskId);
                    //if (task == null)
                    //{
                    //    results.Add($"Task ID {request.TaskId} không tồn tại.");
                    //    continue;
                    //}

                    //// Kiểm tra Status của Task phải là 3
                    //if (task.Status != 3)
                    //{
                    //    results.Add($"Task ID {request.TaskId} không có trạng thái hợp lệ để tạo lịch trình.");
                    //    continue;
                    //}

                    //// Kiểm tra ngày của Schedule không được vượt quá ngày của Task
                    //if (request.Date > task.EndDate)
                    //{
                    //    results.Add($"Ngày của lịch trình vượt quá ngày kết thúc của Task ID {request.TaskId}.");
                    //    continue;
                    //}

                    //// 3. Lấy thông tin OrderDetail và MartyrGrave
                    //var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(
                    //    filter: od => od.DetailId == task.DetailId,
                    //    includeProperties: "MartyrGrave"
                    //);

                    //var orderDetail = orderDetails.SingleOrDefault();
                    //if (orderDetail == null || orderDetail.MartyrGrave == null)
                    //{
                    //    results.Add($"OrderDetail hoặc MartyrGrave không tồn tại cho Task ID {request.TaskId}.");
                    //    continue;
                    //}

                    //// 4. Kiểm tra AreaId của nhân viên và MartyrGrave
                    //if (account.AreaId != orderDetail.MartyrGrave.AreaId)
                    //{
                    //    results.Add($"Nhân viên không thuộc khu vực của Task ID {request.TaskId}.");
                    //    continue;
                    //}

                    // 5. Kiểm tra Slot có tồn tại không
                    var slot = await _unitOfWork.SlotRepository.GetByIDAsync(request.SlotId);
                    if (slot == null)
                    {
                        results.Add($"Slot ID {request.SlotId} không tồn tại.");
                        continue;
                    }

                   
                    var existingSchedule = (await _unitOfWork.ScheduleRepository.GetAsync(
                        s => s.Date == request.Date && s.SlotId == request.SlotId
                    )).FirstOrDefault();

                    if (existingSchedule != null)
                    {
                        results.Add($"Lịch trình này đã tồn tại.");
                        continue;
                    }

                    // 7. Tạo mới lịch trình nếu các điều kiện đã được thỏa mãn
                    var newSchedule = _mapper.Map<Schedule>(request);
                    newSchedule.Status = 1; // Đặt mặc định là active
                    newSchedule.AccountId = accountId;
                    await _unitOfWork.ScheduleRepository.AddAsync(newSchedule);

                    results.Add($"Lịch trình đã được tạo thành công.");
                }

                return results;
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
        }






        public async Task<List<ScheduleDTOResponse>> GetScheduleByAccountId(int accountId)
        {
            // Lấy danh sách Schedule của accountId, bao gồm Slot và Account
            var schedules = await _unitOfWork.ScheduleRepository.GetAsync(
                filter: s => s.AccountId == accountId,
                includeProperties: "Slot,Account"
            );

            var scheduleResponses = new List<ScheduleDTOResponse>();

            foreach (var schedule in schedules)
            {
                // Lấy Task dựa vào TaskId trong mỗi Schedule
                var task = await _unitOfWork.TaskRepository.GetByIDAsync(schedule.TaskId);

                if (task != null)
                {
                    // Lấy OrderDetail của Task và bao gồm các navigation properties cần thiết
                    var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(
                        filter: od => od.DetailId == task.DetailId,
                        includeProperties: "Service,MartyrGrave.Location"
                    );

                    var orderDetailEntity = orderDetail.FirstOrDefault();

                    // Xác định thông tin Location nếu có dữ liệu
                    var martyrGrave = orderDetailEntity?.MartyrGrave;
                    var location = martyrGrave != null && martyrGrave.Location != null
                        ? $"K{martyrGrave.Location.AreaNumber}-R{martyrGrave.Location.RowNumber}-{martyrGrave.Location.MartyrNumber}"
                        : "Location Not Available";

                    // Tạo response cho từng schedule
                    var response = new ScheduleDTOResponse
                    {
                        ScheduleId = schedule.ScheduleId,
                        AccountId = schedule.AccountId,
                        SlotId = schedule.SlotId,
                        TaskId = schedule.TaskId,
                        //FullName = schedule.Account.FullName,
                        SlotName = schedule.Slot.SlotName,
                        StartTime = schedule.Slot.StartTime,
                        EndTime = schedule.Slot.EndTime,
                        ServiceName = orderDetailEntity?.Service?.ServiceName,
                        Location = location
                    };

                    scheduleResponses.Add(response);
                }
            }

            return scheduleResponses;
        }



        public async Task<string> UpdateSchedule(int scheduleId, UpdateScheduleDTORequest request)
        {
            // Lấy thông tin Schedule
            var schedule = await _unitOfWork.ScheduleRepository.GetByIDAsync(scheduleId);
            if (schedule == null)
            {
                return "Schedule không tồn tại.";
            }

            // Kiểm tra TaskId của Schedule
            var task = await _unitOfWork.TaskRepository.GetByIDAsync(schedule.TaskId);
            if (task == null)
            {
                return "Task không tồn tại";
            }

            // Kiểm tra ngày của Schedule không được vượt quá ngày kết thúc của Task
            if (request.Date > DateOnly.FromDateTime(task.EndDate))
            {
                return "Ngày của lịch trình vượt quá ngày kết thúc của Task.";
            }

            // Lấy thông tin OrderDetail và MartyrGrave để kiểm tra khu vực
            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(
                filter: od => od.DetailId == task.DetailId,
                includeProperties: "MartyrGrave"
            );

            var orderDetail = orderDetails.SingleOrDefault();
            if (orderDetail == null || orderDetail.MartyrGrave == null)
            {
                return "OrderDetail hoặc MartyrGrave không tồn tại.";
            }

            // Kiểm tra AreaId của nhân viên và MartyrGrave
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(schedule.AccountId);
            if (account == null || account.AreaId != orderDetail.MartyrGrave.AreaId)
            {
                return "Nhân viên không thuộc khu vực của Task.";
            }

            // Kiểm tra xem Slot có tồn tại không
            var slot = await _unitOfWork.SlotRepository.GetByIDAsync(request.SlotId);
            if (slot == null)
            {
                return "Slot không tồn tại.";
            }

            // Kiểm tra số lượng Task trong Slot cho cùng một ngày không vượt quá 3
            var existingTasksInSlot = await _unitOfWork.ScheduleRepository.GetAsync(
                s => s.Date == request.Date && s.SlotId == request.SlotId && s.ScheduleId != scheduleId
            );

            if (existingTasksInSlot.Count() >= 3)
            {
                return $"Slot {slot.SlotName} vào ngày {request.Date.ToShortDateString()} đã đạt đến giới hạn tối đa là 3 Task.";
            }

            // Cập nhật thông tin Schedule dựa trên dữ liệu từ request
            schedule.Date = request.Date;
            schedule.SlotId = request.SlotId;
            schedule.Description = request.Description;

            // Cập nhật thông tin Schedule
            await _unitOfWork.ScheduleRepository.UpdateAsync(schedule);
            await _unitOfWork.SaveAsync();

            return "Cập nhật Schedule thành công.";
        }


        public async Task<string> DeleteSchedule(int scheduleId)
        {
            // Lấy thông tin Schedule
            var schedule = await _unitOfWork.ScheduleRepository.GetByIDAsync(scheduleId);
            if (schedule == null)
            {
                return "Schedule không tồn tại.";
            }

            // Xóa Schedule
            await _unitOfWork.ScheduleRepository.DeleteAsync(scheduleId);
            await _unitOfWork.SaveAsync();

            return "Xóa Schedule thành công.";
        }

        public async Task<ScheduleDTOResponse> GetScheduleById(int scheduleId)
        {
            // Lấy Schedule dựa trên scheduleId, bao gồm Slot và Account
            var schedule = await _unitOfWork.ScheduleRepository.GetAsync(
                filter: s => s.ScheduleId == scheduleId,
                includeProperties: "Slot,Account"
            );

            var scheduleEntity = schedule.FirstOrDefault();
            if (scheduleEntity == null)
            {
                throw new KeyNotFoundException("Schedule not found.");
            }

            // Lấy Task dựa vào TaskId của Schedule
            var task = await _unitOfWork.TaskRepository.GetByIDAsync(scheduleEntity.TaskId);
            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            // Lấy OrderDetail của Task và bao gồm các navigation properties cần thiết
            var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(
                filter: od => od.DetailId == task.DetailId,
                includeProperties: "Service,MartyrGrave.Location"
            );

            var orderDetailEntity = orderDetail.FirstOrDefault();

            // Xác định thông tin Location nếu có dữ liệu
            var martyrGrave = orderDetailEntity?.MartyrGrave;
            var location = martyrGrave != null && martyrGrave.Location != null
                ? $"K{martyrGrave.Location.AreaNumber}-R{martyrGrave.Location.RowNumber}-{martyrGrave.Location.MartyrNumber}"
                : "Location Not Available";

            // Tạo response với các thuộc tính giống GetScheduleByAccountId
            var response = new ScheduleDTOResponse
            {
                ScheduleId = scheduleEntity.ScheduleId,
                AccountId = scheduleEntity.AccountId,
                SlotId = scheduleEntity.SlotId,
                TaskId = scheduleEntity.TaskId,
                //FullName = scheduleEntity.Account.FullName,
                SlotName = scheduleEntity.Slot.SlotName,
                StartTime = scheduleEntity.Slot.StartTime,
                EndTime = scheduleEntity.Slot.EndTime,
                ServiceName = orderDetailEntity?.Service?.ServiceName,
                Location = location
            };

            return response;
        }

    }
}