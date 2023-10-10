using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Services;

namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            this.wishlistService = wishlistService;
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<Provider.Entities.Response<int>> Post(int productId)
        {
            try
            {
                int customerId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                int id = await wishlistService.Post(productId, customerId);

                Provider.Entities.Response<int> result = new Provider.Entities.Response<int>()
                {
                    Data = id,
                    Message = "Product successfully added to the wishlist",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return result;

            }
            catch (Exception e)
            {
                Provider.Entities.Response<int> result = new Provider.Entities.Response<int>()
                {
                    Data = 0,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return result;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<Provider.Entities.Response<WishlistDto>> GetWishlist()
        {
            try
            {
                int? customerId = Convert.ToInt32(HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value);

                var result = await wishlistService.GetWishlist(customerId??0);
                if (result != null)
                    return new Provider.Entities.Response<WishlistDto>()
                    {
                        Data = result,
                        Message = "Wishlist fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Provider.Entities.Response<WishlistDto>()
                    {
                        Data = null,
                        Message = "There is nothing in your wishlist",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<WishlistDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<Provider.Entities.Response<bool>> Delete(int id)
        {
            try
            {
                int customerId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);
                bool ans = await wishlistService.Delete(id,customerId);
                if (ans)
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = true,
                        Message = "Product in wishlist deleted successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = true,
                        Message = "Product in a wishlist do not deleted",
                        StatusCode = System.Net.HttpStatusCode.InternalServerError
                    };
            }
            catch (Exception e)
            {
                Provider.Entities.Response<bool> response = new Provider.Entities.Response<bool>()
                {
                    Data = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return response;
            }
        }

    }
}
