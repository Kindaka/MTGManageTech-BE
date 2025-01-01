using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

namespace MartyrGraveManagement_BAL.BackgroundServices.Implements
{
    public class OrderBackgroundService : IOrderBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderBackgroundService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CheckExpiredOrderPayment()
        {

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var expiredOrders = await _unitOfWork.OrderRepository.GetAsync(order =>
                            order.Status == 0 && order.OrderDate.AddMinutes(10) <= DateTime.Now);

                    if (expiredOrders.Any())
                    {
                        foreach (var order in expiredOrders)
                        {
                            // Cập nhật trạng thái đơn hàng thành đã hủy (status 2)
                            order.Status = 2;
                            await _unitOfWork.OrderRepository.UpdateAsync(order);
                        }

                        // Lưu tất cả thay đổi
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log error
                    Console.WriteLine($"Error in CheckExpiredOrderPayment: {ex.Message}");
                }
            }


        }
    }
}
