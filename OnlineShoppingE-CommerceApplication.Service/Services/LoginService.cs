using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class LoginService : ILoginService
    {
        private readonly OnlineShoppingDbContext dbContext;
        private readonly IConfiguration config;
        public LoginService(IConfiguration config, OnlineShoppingDbContext dbContext)
        {
            this.config = config;
            this.dbContext = dbContext;
        }


        public string GenerateToken(Provider.Entities.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Role,user.Role.ToString()),
                new Claim("Email",user.Email),
                new Claim("Id",user.Id.ToString())

            };
            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //To authenticate user
        public async Task<LoginResponse?> Authenticate(Login login)
        {
            Provider.Entities.User? user = await dbContext.User.FirstOrDefaultAsync(x => x.Email == login.Email && x.Password == login.Password && x.IsActive == true);

            if (user != null)
            {
                LoginResponse response = new LoginResponse()
                {
                    Token = GenerateToken(user),
                    UserInfo = new Provider.DTOs.UserDto()
                    {
                       
                        Role = user.Role,
                        Id = user.Id,
                        Mobile= user.Mobile,
                        Address = user.Address,
                        City = user.City,
                        Email = user.Email,
                        Name= user.Name
       
                    }
                };
                return response;
            }
            return null;
        }

    }
}
