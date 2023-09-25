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
    public class ColourController : ControllerBase
    {
        private readonly IColourService colourService;
        public ColourController(IColourService colourService)
        {
            this.colourService = colourService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<Response<int>> Post([FromForm] Colour colour)
        {
            try
            {
                int id = await colourService.Post(colour);
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
        public async Task<Response<ColourDto>> GetAll(QueryBase query)
        {
            try
            {
                var result = await colourService.GetAll(query);
                if (result != null)
                {
                    Response<ColourDto> colours = new Response<ColourDto>()
                    {
                        Data = result,
                        Message = "List of colours fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return colours;
                }
                else
                {
                    Response<ColourDto> colours = new Response<ColourDto>()
                    {
                        Data = null,
                        Message = "No colour found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
                    return colours;
                }
            }
            catch (Exception e)
            {
                Response<ColourDto> colours = new Response<ColourDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
                return colours;

            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<Response<bool>> Update([FromForm] Colour colour, int id)
        {
            try
            {
                if (!colour.Id.Equals(id))
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "The id does not match with the colour to be updated",
                        Data = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return result;

                }
                else
                {
                    bool ans = await colourService.Update(colour, id);
                    if (ans)
                        return new Response<bool>()
                        {
                            Message = "Colour updated",
                            Data = true,
                            StatusCode = System.Net.HttpStatusCode.OK
                        };
                    else
                        return new Response<bool>()
                        {
                            Message = "Colour cant updated as already exists",
                            Data = false,
                            StatusCode = System.Net.HttpStatusCode.InternalServerError
                        };
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
                bool ans = await colourService.UpdateStatus(id, status);
                if (ans)
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Colour's status updated",
                        Data = true,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Colour's id  not found",
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
        public async Task<Response<ColourDetailDto>> GetById(int id)
        {
            try
            {
                var result = await colourService.GetById(id);
                if (result != null)
                    return new Response<ColourDetailDto>()
                    {
                        Data = result,
                        Message = "Colour details fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Response<ColourDetailDto>()
                    {
                        Data = null,
                        Message = "No colour found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
            }
            catch (Exception e)
            {
                return new Response<ColourDetailDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
    }
}
