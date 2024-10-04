using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.OrdersDTOs;
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
    public class OdersService : IOdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OdersService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrdersDTOResponse>> GetAll()
        {
            var items = await _unitOfWork.OrderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrdersDTOResponse>>(items);
        }
        public async Task<IEnumerable<OrdersDTOResponse>> GetById(int id)
        {
            var items = await _unitOfWork.OrderRepository.GetByIDAsync(id);
            return _mapper.Map<IEnumerable<OrdersDTOResponse>>(items);
        }

        public async Task<OrdersDTOResponse> CreateAsync(OrdersDTORequest ordersDTO)
        {
            // Kiểm tra AccountID có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(ordersDTO.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException("AccountID does not exist.");
            }
            

            // Tạo thực thể từ DTO
            var item = _mapper.Map<Order>(ordersDTO);

            // Thêm Cart vào cơ sở dữ liệu
            await _unitOfWork.OrderRepository.AddAsync(item);
            await _unitOfWork.SaveAsync();

            // Trả về DTO response
            return _mapper.Map<OrdersDTOResponse>(item);
        }

        public async Task<OrdersDTOResponse> UpdateAsync(int id, OrdersDTORequest ordersDTO)
        {
            // Kiểm tra AreaId có tồn tại không
            var account = await _unitOfWork.AreaRepository.GetByIDAsync(ordersDTO.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException("AccountID does not exist.");
            }
            

            var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
            if (order == null)
            {
                return null;
            }

            // Cập nhật các thuộc tính từ DTO sang thực thể
            _mapper.Map(ordersDTO, order);

            // Cập nhật thông tin vào cơ sở dữ liệu
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.SaveAsync();

            // Trả về kết quả cập nhật
            return _mapper.Map<OrdersDTOResponse>(order);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIDAsync(id);
            if (order == null)
            {
                return false;
            }
            // Cập nhật tình trạng
            order.Status = 0;

            //Cập nhật thông tin vào cơ sở dữ liệu
            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
