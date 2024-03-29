﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Service.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        private readonly ISizeService sizeService;
        public SizeController(ISizeService sizeService)
        {
            this.sizeService = sizeService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<Response<int>> Post(Size size)
        {
            try
            {
                int id = await sizeService.Post(size);
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
                        Message = "Save fail",
                        StatusCode = System.Net.HttpStatusCode.InternalServerError
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
        [HttpPost]
        public async Task<Response<SizeDto>> GetAll(QueryBase query)
        {
            try
            {
                var result = await sizeService.GetAll(query);
                if (result != null)
                    return new Response<SizeDto>()
                    {
                        Data = result,
                        Message = "List of size fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Response<SizeDto>()
                    {
                        Data = null,
                        Message = "No size found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
            }
            catch (Exception e)
            {
                return new Response<SizeDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<Response<bool>> Update(Size size, int id)
        {
            try
            {
                if (!size.Id.Equals(id))
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "The id does not match with the size to be updated",
                        Data = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return result;

                }
                else
                {

                    bool ans = await sizeService.Update(size, id);
                    if (ans)
                    {
                        Response<bool> result = new Response<bool>()
                        {
                            Message = "size updated",
                            Data = true,
                            StatusCode = System.Net.HttpStatusCode.OK
                        };
                        return result;
                    }
                    else
                    {
                        Response<bool> result = new Response<bool>()
                        {
                            Message = "size cannot updated",
                            Data = false,
                            StatusCode = System.Net.HttpStatusCode.InternalServerError
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
                bool ans = await sizeService.UpdateStatus(id, status);
                if (ans)
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Size's status updated",
                        Data = true,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Size's id  not found",
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

        [HttpGet("{id}")]
        public async Task<Response<SizeDetailDto>> GetById(int id)
        {
            try
            {
                var result = await sizeService.GetById(id);
                if (result != null)
                    return new Response<SizeDetailDto>()
                    {
                        Data = result,
                        Message = "Size details fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Response<SizeDetailDto>()
                    {
                        Data = null,
                        Message = "No size found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
            }
            catch (Exception e)
            {
                return new Response<SizeDetailDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }

    }
}
