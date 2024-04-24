using OnlineShoppingECommerceApplication.Frontend.Models;

namespace OnlineShoppingECommerceApplication.Frontend.Interfaces
{
    public interface IRegisterService
    {
        Task<bool> Register(UserDto user);
    }
}
