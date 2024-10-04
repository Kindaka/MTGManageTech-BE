using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
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
    public class CartItemsService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartItemsDTOResponse>> GetAllCartItems()
        {
            var cart = await _unitOfWork.CartItemRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CartItemsDTOResponse>>(cart);
        }
        public async Task<IEnumerable<CartItemsDTOResponse>> GetAllCartItemById(int id)
        {
            var cart = await _unitOfWork.CartItemRepository.GetByIDAsync(id);
            return _mapper.Map<IEnumerable<CartItemsDTOResponse>>(cart);
        }

        public async Task<CartItemsDTOResponse> CreateCartItemsAsync(CartItemsDTORequest cartItemsDTO)
        {
            // Kiểm tra xem AccountID có tồn tại không
            var account = await _unitOfWork.AccountRepository.GetByIDAsync(cartItemsDTO.AccountId);
            if (account == null)
            {
                throw new KeyNotFoundException("AccountID does not exist.");
            }

            // Kiểm tra ServiceID có tồn tại không
            var service = await _unitOfWork.ServiceRepository.GetByIDAsync(cartItemsDTO.ServiceId);
            if (service == null)
            {
                throw new KeyNotFoundException("ServiceID does not exist.");
            }

            // Tìm MartyrGrave dựa trên CustomerCode của tài khoản
            var martyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.CustomerCode == account.CustomerCode)).FirstOrDefault();
            if (martyrGrave != null)
            {
                // Tạo thực thể CartItem từ DTO và đặt Status = 1
                var cart = _mapper.Map<CartItem>(cartItemsDTO);
                cart.MartyrId = martyrGrave.MartyrId;
                cart.Status = true;  // Đặt Status là 1 (true)

                // Thêm CartItem vào cơ sở dữ liệu
                await _unitOfWork.CartItemRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();

                // Trả về DTO response
                return _mapper.Map<CartItemsDTOResponse>(cart);
            }
            else
            {
                throw new KeyNotFoundException("MartyrGrave for the account's CustomerCode does not exist.");
            }
        }



        public async Task<bool> DeleteCartItemsAsync(int id)
        {
            var cart = await _unitOfWork.CartItemRepository.GetByIDAsync(id);
            if (cart == null)
            {
                return false;
            }
            // Cập nhật tình trạng
            cart.Status = false;

            //Cập nhật thông tin vào cơ sở dữ liệu
            await _unitOfWork.CartItemRepository.UpdateAsync(cart);
            await _unitOfWork.SaveAsync();
            return true;
        }

    }
}
