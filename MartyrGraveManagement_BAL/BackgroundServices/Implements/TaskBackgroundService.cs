using MartyrGraveManagement.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement.BackgroundServices.Implements
{
    public class TaskBackgroundService : ITaskBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckExpiredTasks()
        {
            while (true)  // Vòng lặp liên tục để kiểm tra task hết hạn
            {
                // Lấy các task có trạng thái "đang thực hiện" (status 0123) và hết hạn
                var expiredTasks = await _unitOfWork.TaskRepository.GetAsync(task =>
                           task.Status < 4 && task.Status >= 0 && task.EndDate <= DateTime.Now);

                if (expiredTasks.Any())
                {
                    foreach (var task in expiredTasks)
                    {
                        // Cập nhật trạng thái task thành thất bại (status 5)
                        task.Status = 5;

                        // Lấy Order liên quan đến task và cập nhật trạng thái Order thành thất bại
                        var order = await _unitOfWork.OrderRepository.GetByIDAsync(task.OrderId);
                        if (order != null)
                        {
                            order.Status = 5; // Cập nhật trạng thái Order thành thất bại
                            await _unitOfWork.OrderRepository.UpdateAsync(order);
                        }

                        // Cập nhật task
                        await _unitOfWork.TaskRepository.UpdateAsync(task);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.SaveAsync();
                }

                // Chờ 10 giây trước khi kiểm tra lại
                await Task.Delay(10000);
            }
        }
    }
}
