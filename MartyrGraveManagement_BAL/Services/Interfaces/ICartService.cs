using MartyrGraveManagement_BAL.ModelViews.CartItemsDTOs;
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
        Task<CartItemsDTOResponse> CreateCartItemsAsync(CartItemsDTORequest cartItemsDTO);
        Task<bool> DeleteCartItemsAsync(int id);

        Task<List<CartItemGetByCustomerDTOResponse>> GetCartItemsByAccountId(int accountId);

    }
}
