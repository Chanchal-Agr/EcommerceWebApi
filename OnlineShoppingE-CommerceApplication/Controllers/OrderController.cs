using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Services;
using Azure;

namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;
        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<Provider.Entities.Response<int>> Post(OrderDto orderDto)
        {
            try
            {
                int customerId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                int id = await orderService.Post(orderDto, customerId);
                if (id > 0)
                {
                    Provider.Entities.Response<int> result = new Provider.Entities.Response<int>()
                    {
                        Data = id,
                        Message = "Order placed",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Provider.Entities.Response<int> result = new Provider.Entities.Response<int>()
                    {
                        Data = id,
                        Message = "Order can't placed",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;

                }
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
        [HttpPut("{orderId}/{status}")]
        [Authorize]
        public async Task<Provider.Entities.Response<bool>> UpdateStatus(int orderId, OrderStatus status)
        {
            try
            {
                bool ans = await orderService.UpdateStatus(orderId, status);
                if (ans)
                {
                    Provider.Entities.Response<bool> result = new Provider.Entities.Response<bool>()
                    {
                        Data = true,
                        Message = "Order status updated",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Provider.Entities.Response<bool> result = new Provider.Entities.Response<bool>()
                    {
                        Data = false,
                        Message = "Order status can't updated",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
            }
            catch (Exception e)
            {
                Provider.Entities.Response<bool> result = new Provider.Entities.Response<bool>()
                {
                    Data = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return result;

            }

        }

        [HttpPost]
        [Authorize]
        public async Task<Provider.Entities.Response<OrderInfoDto>> GetOrders(OrderQuery query)
        {
            try
            {
                var result = await orderService.GetOrders(query);
                if (result != null)
                {
                    return new Provider.Entities.Response<OrderInfoDto>()
                    {
                        Data = result,
                        Message = "List of orders fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    return new Provider.Entities.Response<OrderInfoDto>()
                    {
                        Data = null,
                        Message = "No order found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
                }
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<OrderInfoDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<Provider.Entities.Response<OrderDetailDto>> GetById(int orderId)
        {
            try
            {
                var result = await orderService.GetById(orderId);
                if (result != null)
                {
                    return new Provider.Entities.Response<OrderDetailDto>()
                    {
                        Data = result,
                        Message = "Order details fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    return new Provider.Entities.Response<OrderDetailDto>()
                    {
                        Data = null,
                        Message = "No order found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
                }
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<OrderDetailDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }

        [Authorize]
        [HttpGet("{customerId}/{orderId}")]
        public async Task<IActionResult> GenerateInvoice(int customerId, int orderId)
        {
            var ss = await orderService.GenerateInvoice(customerId, orderId);
            if(ss!=null)
               return File(ss.Item1, "application/pdf", ss.Item2);
           
            return NotFound();
        }
    }
}
