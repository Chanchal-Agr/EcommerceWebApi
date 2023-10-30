using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface ICartService
    {
        Task<int> Post(CartDto cartDto,int customerId);
        Task<CartInfoDto> GetDetails(int customerId);
        Task<bool> Delete(int customerId,int id);
        Task<bool> Update(int customerId, int id,int quantity);
        Task<int> GetCartCount(int customerId);
    }
}
