using OnlineShoppingE_CommerceApplication.Provider.Entities;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using Microsoft.AspNetCore.Authorization;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Service.Services;

namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost]
        public async Task<Response<int>> PostCustomer(User customer)
        {
            try
            {
                customer.Role = Roles.Customer;
                int id = await userService.Post(customer);
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
                        Message = "Save fail,Email or Mobile number is already registered",
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
        public async Task<Response<int>> PostEmployee(User employee)
        {
            try
            {
                employee.Role = Roles.Admin;
                int id = await userService.Post(employee);
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
                        Message = "Save fail,Email or Mobile number is already registered",
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
        [HttpGet("{id}")]
        [Authorize]
        public async Task<Provider.Entities.Response<UserDto>> Profile(int id)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);
            
                var result = await userService.Profile(id,userId);
                if (result != null)
                    return new Provider.Entities.Response<UserDto>()
                    {
                        Data = result,
                        Message = "Profile fetched successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Provider.Entities.Response<UserDto>()
                    {
                        Data = null,
                        Message = "User not found",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<UserDto>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
        [HttpPut]
        [Authorize]
        public async Task<Provider.Entities.Response<bool>> UpdatePassword(UserPasswordDto info)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                bool result = await userService.UpdatePassword(info,userId);
                if (result )
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = result,
                        Message = "Password updated successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = result,
                        Message = "Existing password does not match",
                        StatusCode = System.Net.HttpStatusCode.NotModified

                    };
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<bool>()
                {
                    Data = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
        [HttpPut]
        [Authorize]
        public async Task<Provider.Entities.Response<bool>> UpdateProfile(UserInfoDto info)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                bool result = await userService.UpdateProfile(info, userId);
                if (result)
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = result,
                        Message = "Profile updated successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = result,
                        Message = "User whose profile to be updated is not found",
                        StatusCode = System.Net.HttpStatusCode.NotModified

                    };
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<bool>()
                {
                    Data = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
        [HttpGet("{mobileNo}")]
        [Authorize]
        public async Task<Provider.Entities.Response<string>> GenerateOtp(string mobileNo)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                var otp = await userService.GenerateOtp(mobileNo,userId);
                if (otp != null)
                    return new Provider.Entities.Response<string>()
                    {
                        Data = otp,
                        Message = "Otp generated successfully to update password",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Provider.Entities.Response<string>()
                    {
                        Data = null,
                        Message = "Entered mobile number does not registered with your profile",
                        StatusCode = System.Net.HttpStatusCode.NoContent

                    };
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<string>()
                {
                    Data = null,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
        [HttpPut("{otp}/{newPassword}")]
        [Authorize]
        public async Task<Provider.Entities.Response<bool>> ForgotPassword(string otp,string newPassword)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.User.Claims.First(c => c.Type == "Id").Value);

                bool result = await userService.ForgotPassword(otp,newPassword,userId);
                if (result)
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = result,
                        Message = "Password updated successfully",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                else
                    return new Provider.Entities.Response<bool>()
                    {
                        Data = result,
                        Message = "Invalid Otp , Password can't updated",
                        StatusCode = System.Net.HttpStatusCode.NotModified

                    };
            }
            catch (Exception e)
            {
                return new Provider.Entities.Response<bool>()
                {
                    Data = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError

                };
            }
        }
    }
}
