using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Services;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;

namespace OnlineShoppingE_CommerceApplication.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductVariantController : ControllerBase
{
    private readonly IProductVariantService productVariantService;
    public ProductVariantController(IProductVariantService productVariantService)
    {
        this.productVariantService = productVariantService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<Response<bool>> AddProductVariantList(List<ProductVariantDto> productVariant)
    {
        try
        {
            bool ans = await productVariantService.Post(productVariant);
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
                    Message = "Save fail as variant already exists",
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/{status}")]
    public async Task<Response<bool>> UpdateStatus(int id, bool status)
    {
        try
        {
            bool ans = await productVariantService.UpdateStatus(id, status);
            if (ans)
            {
                Response<bool> result = new Response<bool>()
                {
                    Message = "Product variant's status updated",
                    Data = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return result;
            }
            else
            {
                Response<bool> result = new Response<bool>()
                {
                    Message = "Product variant's id not found",
                    Data = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return result;
            }
        }
        catch (Exception e)
        {

            Response<bool> result = new Response<bool>()
            {
                Message = e.Message,
                Data = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
            return result;
        }
    }



}
