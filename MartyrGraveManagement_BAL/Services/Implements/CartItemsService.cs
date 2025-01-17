using AutoMapper;
using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;

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

        public async Task<(List<CartItemsDTOResponse>, List<string>)> CreateCartItemsAsync(List<CartItemsDTORequest> cartItemsDTOs)
        {
            var responseList = new List<CartItemsDTOResponse>();
            var successMessages = new List<string>(); // Danh sách thông báo thành công

            foreach (var cartItemsDTO in cartItemsDTOs)
            {
                try
                {
                    // Kiểm tra AccountID có tồn tại không
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(cartItemsDTO.AccountId);
                    if (account == null)
                    {
                        throw new KeyNotFoundException($"AccountID {cartItemsDTO.AccountId} does not exist.");
                    }

                    // Kiểm tra ServiceID có tồn tại không
                    var service = await _unitOfWork.ServiceRepository.GetByIDAsync(cartItemsDTO.ServiceId);
                    if (service == null)
                    {
                        throw new KeyNotFoundException($"ServiceID {cartItemsDTO.ServiceId} does not exist.");
                    }

                    // Lấy tên dịch vụ từ thực thể Service
                    var serviceName = service.ServiceName;

                    // Tìm MartyrGrave dựa trên MartyrId
                    var martyrGrave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrId == cartItemsDTO.MartyrId)).FirstOrDefault();
                    if (martyrGrave == null)
                    {
                        throw new KeyNotFoundException($"MartyrGrave for MartyrID {cartItemsDTO.MartyrId} does not exist.");
                    }

                    // Lấy thông tin Name từ MartyrGraveInformation
                    var martyrInfo = (await _unitOfWork.MartyrGraveInformationRepository.FindAsync(m => m.MartyrId == cartItemsDTO.MartyrId)).FirstOrDefault();
                    var martyrName = martyrInfo?.Name ?? "Unknown Martyr";

                    // Kiểm tra nếu GraveService tồn tại cho MartyrId và ServiceId
                    var graveService = (await _unitOfWork.GraveServiceRepository.FindAsync(gs =>
                        gs.MartyrId == cartItemsDTO.MartyrId &&
                        gs.ServiceId == cartItemsDTO.ServiceId)).FirstOrDefault();
                    if (graveService == null)
                    {
                        throw new InvalidOperationException($"Không thể thêm dịch vụ {serviceName} vào giỏ hàng vì nó không khả dụng cho Liệt sĩ {martyrName}.");
                    }

                    // Kiểm tra nếu mục đã tồn tại trong giỏ hàng
                    var existingCartItem = await _unitOfWork.CartItemRepository.FindAsync(c =>
                        c.AccountId == cartItemsDTO.AccountId &&
                        c.ServiceId == cartItemsDTO.ServiceId &&
                        c.MartyrId == cartItemsDTO.MartyrId);
                    if (existingCartItem.Any())
                    {
                        throw new InvalidOperationException($"Service {serviceName} is already in the cart for Martyr grave {martyrName}.");
                    }

                    // Tạo thực thể CartItem
                    var cart = _mapper.Map<CartItemCustomer>(cartItemsDTO);
                    cart.MartyrId = martyrGrave.MartyrId;
                    cart.Status = false; // Đặt Status là false

                    // Thêm CartItem vào cơ sở dữ liệu
                    await _unitOfWork.CartItemRepository.AddAsync(cart);

                    // Tạo response và thêm vào danh sách
                    var response = _mapper.Map<CartItemsDTOResponse>(cart);
                    responseList.Add(response);

                    // Thêm thông báo thành công
                    successMessages.Add($"Dịch vụ '{serviceName}' đã được thêm thành công cho mộ liệt sĩ '{martyrName}'.");
                }
                catch (Exception ex)
                {
                    // Nếu cần, bạn có thể xử lý lỗi từng mục tại đây (nếu không, lỗi sẽ ném ra toàn bộ phương thức)
                    successMessages.Add($"Không thể thêm dịch vụ cho yêu cầu: {ex.Message}");
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu sau khi xử lý tất cả các mục
            await _unitOfWork.SaveAsync();

            return (responseList, successMessages);
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


        public async Task<(List<CartItemGetByCustomerDTOResponse> cartitemList, decimal totalPriceInCart)> GetCartItemsByAccountId(int accountId)
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
                decimal totalPriceInCart = 0;

                foreach (var cartItem in cartItems)
                {
                    var grave = (await _unitOfWork.MartyrGraveRepository.FindAsync(m => m.MartyrId == cartItem.MartyrId)).FirstOrDefault();
                    var graveInfo = (await _unitOfWork.MartyrGraveInformationRepository.FindAsync(m => m.MartyrId == cartItem.MartyrId)).FirstOrDefault();
                    if (grave != null)
                    {
                        // Tạo DTO response cho từng CartItem
                        var cartItemResponse = new CartItemGetByCustomerDTOResponse
                        {
                            CartId = cartItem.CartId,
                            AccountId = cartItem.AccountId,
                            ServiceId = cartItem.ServiceId,
                            MartyrCode = grave.MartyrCode,
                            MartyrId = grave.MartyrId,
                            MartyrName = graveInfo.Name,
                            Status = cartItem.Status
                        };

                        // Lấy thông tin chi tiết của dịch vụ (Service) và ánh xạ sang DTO
                        if (cartItem.Service != null)
                        {
                            cartItemResponse.ServiceView = _mapper.Map<ServiceDtoResponse>(cartItem.Service);

                            //// Kiểm tra nếu CustomerCode của Account và MartyrGrave trùng nhau thì áp dụng giảm giá cho từng dịch vụ
                            //if (grave.AccountId == account.AccountId)
                            //{
                            //    cartItemResponse.ServiceView.Price *= 0.95m; // Giảm giá 5% cho dịch vụ
                            //}

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

        public async Task<(List<CartItemGetByCustomerDTOResponse> cartitemList, decimal totalPriceInCart)> GetCheckoutByAccountId(int accountId)
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
                decimal totalPriceInCart = 0;

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
                            MartyrId = grave.MartyrId,
                            Status = cartItem.Status
                        };

                        // Lấy thông tin chi tiết của dịch vụ (Service) và ánh xạ sang DTO
                        if (cartItem.Service != null)
                        {
                            cartItemResponse.ServiceView = _mapper.Map<ServiceDtoResponse>(cartItem.Service);

                            //// Kiểm tra nếu CustomerCode của Account và MartyrGrave trùng nhau thì áp dụng giảm giá cho từng dịch vụ
                            //if (grave.AccountId == account.AccountId)
                            //{
                            //    cartItemResponse.ServiceView.Price *= 0.95m; // Giảm giá 5% cho dịch vụ
                            //}

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

        public async Task<(List<CartItemGetByGuestDTOResponse> cartitemList, decimal totalPriceInCart)> GetCartForGuest(List<ServiceMartyrGraveDtoRequest> requests)
        {
            try
            {
                var cartItemList = new List<CartItemGetByGuestDTOResponse>();
                decimal totalPriceInCart = 0;
                foreach (var request in requests)
                {
                    var grave = await _unitOfWork.MartyrGraveRepository.GetByIDAsync(request.martyrId);
                    var service = await _unitOfWork.ServiceRepository.GetByIDAsync(request.serviceId);
                    if (grave != null && service != null)
                    {
                        // Tạo DTO response cho từng CartItem
                        var cartItemResponse = new CartItemGetByGuestDTOResponse
                        {
                            ServiceId = service.ServiceId,
                            MartyrCode = grave.MartyrCode,
                            MartyrId = grave.MartyrId,
                        };

                        // Lấy thông tin chi tiết của dịch vụ (Service) và ánh xạ sang DTO
                        if (service != null)
                        {
                            cartItemResponse.ServiceView = _mapper.Map<ServiceDtoResponse>(service);

                            // Tính tổng giá trị trong giỏ hàng
                            totalPriceInCart += cartItemResponse.ServiceView.Price;
                        }

                        // Thêm đối tượng vào danh sách kết quả
                        cartItemList.Add(cartItemResponse);
                    }
                }
                return (cartItemList, totalPriceInCart);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);  // Quản lý lỗi nếu có bất kỳ ngoại lệ nào
            }
        }
    }
}
