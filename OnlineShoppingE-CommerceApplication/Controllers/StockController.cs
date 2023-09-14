using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Services;

namespace OnlineShoppingE_CommerceApplication.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly IStockService stockService;
    public StockController(IStockService stockService)
    {
        this.stockService = stockService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<Response<bool>> AddStock(List<StockDto> stocks)
    {
        try
        {
            bool ans = await stockService.AddStock(stocks);
            if (ans)
            {
                Response<bool> result = new Response<bool>()
                {
                    Data = true,
                    Message = "Save successful",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return result;
            }
            else
            {
                Response<bool> result = new Response<bool>()
                {
                    Data = false,
                    Message = "Save fail",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return result;
            }
        }
        catch (Exception e)
        {
            Response<bool> result = new Response<bool>()
            {
                Data = false,
                Message = e.Message,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
            return result;
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<Provider.Entities.Response<StockInfoDto>> GetDetails(StockQuery query)
    {
        try
        {
            var result = await stockService.GetDetails(query);
            if (result != null)
                return new Provider.Entities.Response<StockInfoDto>()
                {
                    Data = result,
                    Message = "Stock details fetched successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            else
                return new Provider.Entities.Response<StockInfoDto>()
                {
                    Data = null,
                    Message = "No stock found ",
                    StatusCode = System.Net.HttpStatusCode.NoContent
                };
        }
        catch (Exception e)
        {
            return new Provider.Entities.Response<StockInfoDto>()
            {
                Data = null,
                Message = e.Message,
                StatusCode = System.Net.HttpStatusCode.InternalServerError

            };
        }
    }
}

