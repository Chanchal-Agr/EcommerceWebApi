using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Services;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System.Collections.Generic;

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

    //[HttpGet]
    //public async Task<Response<List<ProductInfoDto>>> GetProductVariants()
    //{
    //    try
    //    {
    //        var result = await productVariantService.GetProductVariants();
    //        if (result != null)
    //        {
    //            return new Response<List<ProductInfoDto>>()
    //            {
    //                Data = result,
    //                Message = "List of products fetched successfully",
    //                StatusCode = System.Net.HttpStatusCode.OK
    //            };
    //        }
    //        else
    //        {
    //            return new Response<List<ProductInfoDto>>()
    //            {
    //                Message = "Product not found",
    //                Data = null,
    //                StatusCode = System.Net.HttpStatusCode.NoContent
    //            };
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        return new Response<List<ProductInfoDto>>()
    //        {
    //            Data = null,
    //            Message = e.Message,
    //            StatusCode = System.Net.HttpStatusCode.InternalServerError
    //        };
    //    }
    //}

    [HttpGet("{categoryId}")]
    public async Task<Response<ProductVariantResponseDto>> GetProductVariants(int categoryId)
    {
        try
        {
            int? customerId = Convert.ToInt32(HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value);
            var result = await productVariantService.GetProductVariants(categoryId, customerId ?? 0);
            if (result != null)
            {
                return new Response<ProductVariantResponseDto>()
                {
                    Data = result,
                    Message = "List of products fetched successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else
            {
                return new Response<ProductVariantResponseDto>()
                {
                    Message = "Product not found",
                    Data = null,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }
        }
        catch (Exception e)
        {
            return new Response<ProductVariantResponseDto>()
            {
                Data = null,
                Message = e.Message,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<Response<bool>> UpdateProductVariant(ProductVariantRequestDto variant, int id)
    {
        try
        {
            if (!variant.Id.Equals(id))
            {
                Response<bool> result = new Response<bool>()
                {
                    Message = "The id does not match with the product to be updated",
                    Data = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return result;
            }
            else
            {

                bool ans = await productVariantService.UpdateProductVariant(variant, id);
                if (ans)
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Variant updated",
                        Data = true,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Variant cannot updated as already exists ",
                        Data = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return result;
                }

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
