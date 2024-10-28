using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.MartyrGraveDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using Microsoft.Identity.Client;
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

            var existingCartItem = await _unitOfWork.CartItemRepository.FindAsync(c => c.AccountId == cartItemsDTO.AccountId && c.ServiceId == cartItemsDTO.ServiceId && c.MartyrId == cartItemsDTO.MartyrId);
            if (existingCartItem.Any())
            {
                throw new InvalidOperationException("This service is already in the cart.");
            }

            // Tìm MartyrGrave dựa trên CustomerCode của tài khoản
            var martyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrId == cartItemsDTO.MartyrId)).FirstOrDefault();
            if (martyrGrave != null)
            {

                // Tạo thực thể CartItem từ DTO và đặt Status = 1
                var cart = _mapper.Map<CartItemCustomer>(cartItemsDTO);
                cart.MartyrId = martyrGrave.MartyrId;
                cart.Status = false;  // Đặt Status là (false)

                // Thêm CartItem vào cơ sở dữ liệu
                await _unitOfWork.CartItemRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();

                // Trả về DTO response
                return _mapper.Map<CartItemsDTOResponse>(cart);



            }
            else
            {
                throw new KeyNotFoundException("MartyrGrave does not exist.");
            }
        }



        public async Task<bool> DeleteCartItemsAsync(int id)
        {
            try
            {
                var cartItemToDelete = await _unitOfWork.CartItemRepository.GetByIDAsync(id);
                if (cartItemToDelete != null)
                {
                    await _unitOfWork.CartItemRepository.DeleteAsync(cartItemToDelete);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }



        //public async Task<(List<CartItemGetByCustomerDTOResponse> cartitemList, double totalPriceInCart)> GetCartItemsByAccountId(int accountId)
        //{
        //    try
        //    {
        //        // Lấy danh sách CartItem dựa trên AccountId
        //        var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == accountId, includeProperties: "Service");

        //        // Kiểm tra xem có mặt hàng nào trong giỏ hàng không
        //        if (cartItems == null || !cartItems.Any())
        //        {
        //            return (new List<CartItemGetByCustomerDTOResponse>(), 0);  // Trả về danh sách rỗng nếu không có giỏ hàng
        //        }

        //        // Tạo danh sách CartItemGetByCustomerDTOResponse để chứa kết quả
        //        var cartItemResponses = new List<CartItemGetByCustomerDTOResponse>();
        //        double totalPriceInCart = 0;
        //        foreach (var cartItem in cartItems)
        //        {
        //            var grave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrId == cartItem.MartyrId)).FirstOrDefault();
        //            if (grave != null)
        //            {
        //                // Tạo DTO response cho từng CartItem
        //                var cartItemResponse = new CartItemGetByCustomerDTOResponse
        //                {
        //                    CartId = cartItem.CartId,
        //                    AccountId = cartItem.AccountId,
        //                    ServiceId = cartItem.ServiceId,
        //                    MartyrCode = grave.MartyrCode,
        //                    Status = cartItem.Status
        //                };

        //                // Lấy thông tin chi tiết của dịch vụ (Service) và ánh xạ sang DTO
        //                if (cartItem.Service != null)
        //                {
        //                    cartItemResponse.ServiceView = _mapper.Map<ServiceDtoResponse>(cartItem.Service);
        //                    totalPriceInCart += cartItemResponse.ServiceView.Price;
        //                }

        //                // Thêm đối tượng vào danh sách kết quả
        //                cartItemResponses.Add(cartItemResponse);
        //            }
        //            else
        //            {
        //                throw new KeyNotFoundException("Grave not found.");
        //            }
        //        }
        //        return (cartItemResponses, totalPriceInCart);  // Trả về danh sách giỏ hàng
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);  // Quản lý lỗi nếu có bất kỳ ngoại lệ nào
        //    }
        //}


        public async Task<(List<CartItemGetByCustomerDTOResponse> cartitemList, double totalPriceInCart)> GetCartItemsByAccountId(int accountId)
        {
            try
            {
                // Lấy danh sách CartItem dựa trên AccountId
                var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == accountId, includeProperties: "Service");

                // Kiểm tra xem có mặt hàng nào trong giỏ hàng không
                if (cartItems == null || !cartItems.Any())
                {
                    return (new List<CartItemGetByCustomerDTOResponse>(), 0);  // Trả về danh sách rỗng nếu không có giỏ hàng
                }

                // Lấy thông tin Account
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }

                // Tạo danh sách CartItemGetByCustomerDTOResponse để chứa kết quả
                var cartItemResponses = new List<CartItemGetByCustomerDTOResponse>();
                double totalPriceInCart = 0;

                foreach (var cartItem in cartItems)
                {
                    var grave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrId == cartItem.MartyrId)).FirstOrDefault();
                    if (grave != null)
                    {
                        // Tạo DTO response cho từng CartItem
                        var cartItemResponse = new CartItemGetByCustomerDTOResponse
                        {
                            CartId = cartItem.CartId,
                            AccountId = cartItem.AccountId,
                            ServiceId = cartItem.ServiceId,
                            MartyrCode = grave.MartyrCode,
<<<<<<< HEAD
                            MarrtyrId = grave.MartyrId,
=======
                            MartyrId = grave.MartyrId,
>>>>>>> f78b91f0778e4078ce0d6a62f407f6e06997ba2d
                            Status = cartItem.Status
                        };

                        // Lấy thông tin chi tiết của dịch vụ (Service) và ánh xạ sang DTO
                        if (cartItem.Service != null)
                        {
                            cartItemResponse.ServiceView = _mapper.Map<ServiceDtoResponse>(cartItem.Service);

                            // Kiểm tra nếu CustomerCode của Account và MartyrGrave trùng nhau thì áp dụng giảm giá cho từng dịch vụ
                            if (!string.IsNullOrEmpty(grave.CustomerCode) && grave.CustomerCode == account.CustomerCode)
                            {
                                cartItemResponse.ServiceView.Price *= 0.95; // Giảm giá 5% cho dịch vụ
                            }

                            // Tính tổng giá trị trong giỏ hàng
                            totalPriceInCart += cartItemResponse.ServiceView.Price;
                        }

                        // Thêm đối tượng vào danh sách kết quả
                        cartItemResponses.Add(cartItemResponse);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Grave not found.");
                    }
                }

                return (cartItemResponses, totalPriceInCart);  // Trả về danh sách giỏ hàng và tổng giá
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);  // Quản lý lỗi nếu có bất kỳ ngoại lệ nào
            }
        }

        public async Task<bool> UpdateCartItemStatusByAccountId(int cartItemId, bool status)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItemRepository.GetByIDAsync(cartItemId);
                if (cartItem == null)
                {
                    return false;
                }
                cartItem.Status = status;
                await _unitOfWork.CartItemRepository.UpdateAsync(cartItem);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<CartItemGetByCustomerDTOResponse> cartitemList, double totalPriceInCart)> GetCheckoutByAccountId(int accountId)
        {
            try
            {
                // Lấy danh sách CartItem dựa trên AccountId và chỉ lấy các mục có Status là true
                var cartItems = await _unitOfWork.CartItemRepository.GetAsync(c => c.AccountId == accountId && c.Status == true, includeProperties: "Service");

                // Kiểm tra xem có mặt hàng nào trong giỏ hàng không
                if (cartItems == null || !cartItems.Any())
                {
                    return (new List<CartItemGetByCustomerDTOResponse>(), 0);  // Trả về danh sách rỗng nếu không có giỏ hàng
                }

                // Lấy thông tin Account
                var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                if (account == null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }

                // Tạo danh sách CartItemGetByCustomerDTOResponse để chứa kết quả
                var cartItemResponses = new List<CartItemGetByCustomerDTOResponse>();
                double totalPriceInCart = 0;

                foreach (var cartItem in cartItems)
                {
                    var grave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrId == cartItem.MartyrId)).FirstOrDefault();
                    if (grave != null)
                    {
                        // Tạo DTO response cho từng CartItem
                        var cartItemResponse = new CartItemGetByCustomerDTOResponse
                        {
                            CartId = cartItem.CartId,
                            AccountId = cartItem.AccountId,
                            ServiceId = cartItem.ServiceId,
                            MartyrCode = grave.MartyrCode,
                            Status = cartItem.Status
                        };

                        // Lấy thông tin chi tiết của dịch vụ (Service) và ánh xạ sang DTO
                        if (cartItem.Service != null)
                        {
                            cartItemResponse.ServiceView = _mapper.Map<ServiceDtoResponse>(cartItem.Service);

                            // Kiểm tra nếu CustomerCode của Account và MartyrGrave trùng nhau thì áp dụng giảm giá cho từng dịch vụ
                            if (!string.IsNullOrEmpty(grave.CustomerCode) && grave.CustomerCode == account.CustomerCode)
                            {
                                cartItemResponse.ServiceView.Price *= 0.95; // Giảm giá 5% cho dịch vụ
                            }

                            // Tính tổng giá trị trong giỏ hàng
                            totalPriceInCart += cartItemResponse.ServiceView.Price;
                        }

                        // Thêm đối tượng vào danh sách kết quả
                        cartItemResponses.Add(cartItemResponse);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Grave not found.");
                    }
                }

                return (cartItemResponses, totalPriceInCart);  // Trả về danh sách giỏ hàng và tổng giá
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);  // Quản lý lỗi nếu có bất kỳ ngoại lệ nào
            }
        }
    }
}
