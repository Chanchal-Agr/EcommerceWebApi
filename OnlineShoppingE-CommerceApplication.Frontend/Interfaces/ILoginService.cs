using OnlineShoppingECommerceApplication.Frontend.Models;

namespace OnlineShoppingECommerceApplication.Frontend.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponse?> Authenticate(LoginRequest login);
    }
}
