using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<Response<int>> Post(Category category)
        {
            try
            {
                int id = await categoryService.Post(category);
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
                        Message = "Save fail, Category already exists",
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

        [HttpPost]
        public async Task<Response<CategoryDto>> GetAll(QueryBase query)
        {
            try
            {
                var result = await categoryService.GetAll(query);
                if (result != null)
                {
                    Response<CategoryDto> category = new Response<CategoryDto>()
                    {
                        Data = result,
                        Message = "List of category fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return category;
                }
                else
                {
                    Response<CategoryDto> category = new Response<CategoryDto>()
                    {
                        Data = null,
                        Message = "No category found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
                    return category;
                }
            }
            catch (Exception e)
            {
                Response<CategoryDto> category = new Response<CategoryDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
                return category;

            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<Response<bool>> Update(Category category, int id)
        {
            try
            {
                if (!category.Id.Equals(id))
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "The id does not match with the category to be updated",
                        Data = false,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return result;

                }
                else
                {

                    bool ans = await categoryService.Update(category, id);
                    if (ans)
                    {
                        Response<bool> result = new Response<bool>()
                        {
                            Message = "Category updated",
                            Data = true,
                            StatusCode = System.Net.HttpStatusCode.OK
                        };
                        return result;
                    }
                    else
                    {
                        Response<bool> result = new Response<bool>()
                        {
                            Message = "Category cannot updated",
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
                bool ans = await categoryService.UpdateStatus(id, status);
                if (ans)
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Category's status updated",
                        Data = true,
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                    return result;
                }
                else
                {
                    Response<bool> result = new Response<bool>()
                    {
                        Message = "Category's id  not found",
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
        public async Task<Response<CategoryDetailDto>> GetById(int id)
        {
            try
            {
                var result = await categoryService.GetById(id);
                if (result != null)
                    return new Response<CategoryDetailDto>()
                    {
                        Data = result,
                        Message = "Category fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };

                else
                    return new Response<CategoryDetailDto>()
                    {
                        Data = null,
                        Message = "No category found",
                        StatusCode = System.Net.HttpStatusCode.NoContent
                    };

            }
            catch (Exception e)
            {
                return new Response<CategoryDetailDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<Response<int>> Upsert(Category category)
        {
            try
            {
                int value = await categoryService.Upsert(category);
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
}
