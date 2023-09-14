using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingE_CommerceApplication.Service.Services;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;

namespace OnlineShoppingE_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService loginService;
        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(Login login)
        {
            var user = await loginService.Authenticate(login);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound("user not found");
        }
    }
}
