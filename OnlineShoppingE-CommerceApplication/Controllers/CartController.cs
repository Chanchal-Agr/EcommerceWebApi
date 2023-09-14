using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using Azure;

namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;
        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<Provider.Entities.Response<int>> Post(CartDto cartDto)
        {
            try
            {
                int customerId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                int id = await cartService.Post(cartDto, customerId);

                Provider.Entities.Response<int> result = new Provider.Entities.Response<int>()
                {
                    Data = id,
                    Message = "Item successfully added to the cart",
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
        public async Task<Provider.Entities.Response<CartInfoDto>> GetDetails()
        {
            try
            {
                int id = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                var result = await cartService.GetDetails(id);
                if (result != null)
                {
                    Provider.Entities.Response<CartInfoDto> cart = new Provider.Entities.Response<CartInfoDto>()
                    {
                        Data = result,
                        Message = "Cart details fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return cart;
                }
                else
                {
                    Provider.Entities.Response<CartInfoDto> cart = new Provider.Entities.Response<CartInfoDto>()
                    {
                        Data = null,
                        Message = "There is nothing in your cart",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
                    return cart;
                }

            }
            catch (Exception e)
            {
                Provider.Entities.Response<CartInfoDto> cart = new Provider.Entities.Response<CartInfoDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
                return cart;

            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<Provider.Entities.Response<bool>> Delete(int id)
        {
            try
            {
                int customerId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);
                bool ans = await cartService.Delete(customerId, id);
                if (ans)
                {
                    Provider.Entities.Response<bool> response = new Provider.Entities.Response<bool>()
                    {
                        Data = true,
                        Message = "Deletion of the cart executed successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return response;

                }
                else
                {
                    Provider.Entities.Response<bool> response = new Provider.Entities.Response<bool>()
                    {
                        Data = true,
                        Message = "Cart do not deleted",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return response;

                }
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

        [HttpPut("{id}/{quantity}")]
        [Authorize(Roles = "Customer")]
        public async Task<Provider.Entities.Response<bool>> Update(int id, int quantity)
        {
            try
            {
                var customerId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);
                  bool ans = await cartService.Update(customerId, id, quantity);
                    if (ans)
                    {
                        Provider.Entities.Response<bool> response = new Provider.Entities.Response<bool>()
                        {
                            Data = true,
                            Message = "Cart updated",
                            StatusCode = System.Net.HttpStatusCode.OK
                        };
                        return response;

                    }
                    else
                    {
                        Provider.Entities.Response<bool> response = new Provider.Entities.Response<bool>()
                        {
                            Data = true,
                            Message = " Cart can't updated",
                            StatusCode = System.Net.HttpStatusCode.OK
                        };
                        return response;
                    }
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
