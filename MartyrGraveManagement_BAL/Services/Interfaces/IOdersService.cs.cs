using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface IOdersService
    {
        Task<IEnumerable<OrdersDTOResponse>> GetAll();
        Task<IEnumerable<OrdersDTOResponse>> GetById(int id);
        Task<OrdersDTOResponse> CreateAsync(OrdersDTORequest ordersDTO);

        Task<OrdersDTOResponse> UpdateAsync(int id, OrdersDTORequest ordersDTO);
        Task<bool> DeleteAsync(int id);
    }
}
