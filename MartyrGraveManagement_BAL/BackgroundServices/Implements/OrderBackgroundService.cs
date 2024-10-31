using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            while (true)  
            {
                
                var expiredOrders = await _unitOfWork.OrderRepository.GetAsync(order =>
                    order.Status == 0 && order.OrderDate.AddMinutes(10) <= DateTime.Now);

                if (expiredOrders.Any())
                {
                    foreach (var order in expiredOrders)
                    {
                        // Cập nhật trạng thái task thành thất bại (status 5)
                        order.Status = 2;

                        // Cập nhật task
                        await _unitOfWork.OrderRepository.UpdateAsync(order);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.SaveAsync();
                }
                await Task.Delay(10000);
            }
        }


    }
}
