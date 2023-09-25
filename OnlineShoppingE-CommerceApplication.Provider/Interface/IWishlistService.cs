using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IWishlistService
    {
        Task<int> Post(int productId,int customerId);
        Task<WishlistDto> GetWishlist(int customerId,int userId);
        Task<bool> Delete(int id,int customerId);
    }
}
