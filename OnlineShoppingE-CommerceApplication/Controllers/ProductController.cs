using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Services;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingE_CommerceApplication.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService productService;
    public ProductController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<Response<int>> AddProduct(Product product)
    {
        try
        {
            int id = await productService.AddProduct(product);
            if (id > 0)
                return new Response<int>()
                {
                    Data = id,
                    Message = "Save successful",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            else
                return new Response<int>()
                {
                    Data = id,
                    Message = "Save fail",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
        }
        catch (Exception e)
        {
            return new Response<int>()
            {
                Data = 0,
                Message = e.Message,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };

        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<Response<bool>> Update(Product product, int id)
    {
        try
        {
            if (!product.Id.Equals(id))
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

                bool ans = await productService.Update(product, id);
                if (ans)
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Product updated",
                        Data = true,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Product cannot updated",
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


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/{status}")]
    public async Task<Response<bool>> UpdateStatus(int id, bool status)
    {
        try
        {
            bool ans = await productService.UpdateStatus(id, status);
            if (ans)
            {
                Response<bool> result = new Response<bool>()
                {
                    Message = "Product's status updated",
                    Data = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return result;
            }
            else
            {
                Response<bool> result = new Response<bool>()
                {
                    Message = "Product's id  not found",
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


    [HttpPost]
    public async Task<Response<ProductDto>> QueryProduct(ProductQuery productQuery)
    {
        try
        {
            int? userId = Convert.ToInt32(HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value);
            var result = await productService.QueryProduct(productQuery,userId??0);
            if (result != null)
            {
                Response<ProductDto> list = new Response<ProductDto>()
                {
                    Data = result,
                    Message = "List of products fetched successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return list;
            }
            else
            {
                Response<ProductDto> list = new Response<ProductDto>()
                {
                    Data = null,
                    Message = "No product data found",
                    StatusCode = System.Net.HttpStatusCode.NoContent

                };
                return list;
            }
        }
        catch (Exception e)
        {
            Response<ProductDto> list = new Response<ProductDto>()
            {
                Data = null,
                Message = e.Message,
                StatusCode = System.Net.HttpStatusCode.InternalServerError

            };
            return list;
        }

    }


    [HttpGet("{id}")]
    public async Task<Response<ProductInfoDto>> GetById(int id)
    {
        try
        {
            var result = await productService.GetById(id);
            if (result != null)
            {
                Response<ProductInfoDto> list = new Response<ProductInfoDto>()
                {
                    Data = result,
                    Message = "List of products fetched successfully",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return list;
            }
            else
            {
                Response<ProductInfoDto> list = new Response<ProductInfoDto>()
                {
                    Message = "Product not found",
                    Data = null,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
                return list;
            }
        }
        catch (Exception e)
        {
            Response<ProductInfoDto> list = new Response<ProductInfoDto>()
            {
                Data = null,
                Message = e.Message,
                StatusCode = System.Net.HttpStatusCode.NoContent

            };
            return list;
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<Response<int>> AddProductAndItsVariant([FromForm] ProductAndVariantDto dto)
    {
        try
        {
            int id = await productService.AddProductAndItsVariant(dto);
            if (id > 0)
                return new Response<int>()
                {
                    Data = id,
                    Message = "Save successful",
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            else
                return new Response<int>()
                {
                    Data = id,
                    Message = "Save fail",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
        }
        catch (Exception e)
        {
            return new Response<int>()
            {
                Data = 0,
                Message = e.Message,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<Response<int>> Upsert(Product product)
    {
        try
        {
            int value = await productService.Upsert(product);
            if (value > 0)
                return new Response<int>()
                {
                    Message = "Executed successfully",
                    Data = value,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            else
                return new Response<int>()
                {
                    Message = "Operation failed",
                    Data = value,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

        }
        catch (Exception e)
        {
            return new Response<int>()
            {
                Message = e.Message,
                Data = 0,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };

        }
    }


}