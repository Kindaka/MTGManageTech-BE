using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
using MartyrGraveManagement_BAL.ModelViews.ServiceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.Services.Interfaces
{
    public interface ICartService
    {

        Task<IEnumerable<CartItemsDTOResponse>> GetAllCartItems();
        Task<IEnumerable<CartItemsDTOResponse>> GetAllCartItemById(int id);
        Task<(List<CartItemsDTOResponse>, List<string>)> CreateCartItemsAsync(List<CartItemsDTORequest> cartItemsDTOs);
        Task<bool> DeleteCartItemsAsync(int id);

        Task<(List<CartItemGetByCustomerDTOResponse> cartitemList, double totalPriceInCart)> GetCartItemsByAccountId(int accountId);
        Task<(List<CartItemGetByCustomerDTOResponse> cartitemList, double totalPriceInCart)> GetCheckoutByAccountId(int accountId);
        Task<bool> UpdateCartItemStatusByAccountId(int cartItemId, bool status);
        Task<(List<CartItemGetByGuestDTOResponse> cartitemList, double totalPriceInCart)> GetCartForGuest(List<ServiceMartyrGraveDtoRequest> request);
    }
}
