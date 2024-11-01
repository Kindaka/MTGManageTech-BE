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


        public async Task<string> CreateSchedule(CreateScheduleDTORequest request)
        {
            // 1. Kiểm tra xem tài khoản có phải là nhân viên không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(request.AccountId);
            if (account == null || account.RoleId != 3)
            {
                return "Tài khoản không phải là nhân viên.";
            }

            // 2. Lấy thông tin Task
            var task = await _unitOfWork.TaskRepository.GetByIDAsync(request.TaskId);
            if (task == null)
            {
                return "Task không tồn tại.";
            }

            // Kiểm tra Status của Task phải là 3
            if (task.Status != 3)
            {
                return "Task không có trạng thái hợp lệ để tạo lịch trình.";
            }

            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(
       filter: od => od.DetailId == task.DetailId,
       includeProperties: "MartyrGrave"
   );

            var orderDetail = orderDetails.SingleOrDefault();


            if (orderDetail == null || orderDetail.MartyrGrave == null)
            {
                return "OrderDetail hoặc MartyrGrave không tồn tại.";
            }



            // 4. Kiểm tra AreaId của nhân viên và MartyrGrave
            if (account.AreaId != orderDetail.MartyrGrave.AreaId)
            {
                return "Nhân viên không thuộc khu vực của Task.";
            }

            // 5. Kiểm tra Slot có tồn tại không
            var slot = await _unitOfWork.SlotRepository.GetByIDAsync(request.SlotId);
            if (slot == null)
            {
                return "Slot không tồn tại.";
            }

            // 6. Kiểm tra xem đã tồn tại lịch trình cho nhân viên này trong cùng ngày và Slot chưa
            var existingSchedule = await _unitOfWork.ScheduleRepository.SingleOrDefaultAsync(
                s => s.AccountId == request.AccountId && s.Date.Date == request.Date.Date && s.SlotId == request.SlotId);

            if (existingSchedule != null)
            {
                return "Đã có lịch trình cho nhân viên này trong cùng ngày và Slot.";
            }

            // 7. Tạo mới lịch trình nếu các điều kiện đã được thỏa mãn
            var newSchedule = _mapper.Map<Schedule_Staff>(request);
            newSchedule.Status = 1; // Đặt mặc định là active
            await _unitOfWork.ScheduleRepository.AddAsync(newSchedule);

            return "Lịch trình được tạo thành công.";
        }





    }
}
