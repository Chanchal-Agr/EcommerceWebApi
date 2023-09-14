using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Service.Services;

namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DamageController : ControllerBase
    {
        private readonly IDamageService damageService;
        public DamageController(IDamageService damageService)
        {
            this.damageService = damageService;
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<Response<int>> Post(DamageDto damageStock)
        {
            try
            {
                int id = await damageService.Post(damageStock);
                if (id > 0)
                {
                    Response<int> result = new Response<int>()
                    {
                        Data = id,
                        Message = "Save successful",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Response<int> result = new Response<int>()
                    {
                        Data = id,
                        Message = "Save fail,Stock not available",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return result;
                }
            }
            catch (Exception e)
            {
                Response<int> result = new Response<int>()
                {
                    Data = 0,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return result;
            }
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<Response<bool>> Delete(int id )
        {
            try
            {
                bool result = await damageService.Delete(id);
                if (result)
                {
                    return new Response<bool>()
                    {
                        Data = result,
                        Message = "Stock deleted successful",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    return new Response<bool>()
                    {
                        Data = result,
                        Message = "Stock does not deleted as it is not available",
                        StatusCode = System.Net.HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception e)
            {
                return new Response<bool>()
                {
                    Data = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
               
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<Response<List<DamageStockDto>>> GetAll()
        {
            try
            {
                var result = await damageService.GetAll();
                if (result!=null)
                {
                    return new Response<List<DamageStockDto>>()
                    {
                        Data = result,
                        Message = "Damaged stock fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    return new Response<List<DamageStockDto>>()
                    {
                        Data = null,
                        Message = "Stock does not deleted as it is not available",
                        StatusCode = System.Net.HttpStatusCode.NotFound
                    };
                }
            }
            catch (Exception e)
            {
                return new Response<List<DamageStockDto>>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
