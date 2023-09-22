using OnlineShoppingE_CommerceApplication.Provider.CustomException;
using OnlineShoppingE_CommerceApplication.Service.Database;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System.Reflection.Metadata.Ecma335;

namespace OnlineShoppingE_CommerceApplication.Service.Services
{
    public class UserService : IUserService
    {
        private readonly OnlineShoppingDbContext dbContext;
        public UserService(OnlineShoppingDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> Post(User user)
        {

            user.CreatedAt = DateTime.Now;
            user.Otp = String.Empty;
            if (!(user.Password.Length >= 8 && user.Password.Any(char.IsDigit) &&
            user.Password.Any(char.IsLetter)
            && user.Password.Any(ch => !char.IsLetterOrDigit(ch))))
            {
                throw new PasswordFormatException();
            }

            var email = dbContext.User.FirstOrDefault(x => x.Email == user.Email);
            if (email != null)
                return 0;
            var mobile = dbContext.User.FirstOrDefault(x => x.Mobile == user.Mobile);
            if (mobile != null) return 0;
            dbContext.User.Add(user);
            await dbContext.SaveChangesAsync();
            return user.Id;

        }
        public async Task<UserDto> Profile(int id ,int userId)
        {
            User? data = null;
            if (userId == id || (dbContext.User.FirstOrDefault(x => x.Id == userId && x.Role == Provider.Enums.Roles.Admin)) != null)
            {
                data = dbContext.User.FirstOrDefault(x => x.Id == id && x.IsActive == true);
                if (data == null)
                    return null;
            }
            else
                throw new InvalidUserException();
            UserDto userDto = new UserDto();
            userDto.Id = data.Id;
            userDto.Role = data.Role;
            userDto.Email = data.Email;
            userDto.Address = data.Address;
            userDto.City = data.City;
            userDto.Name = data.Name;
            userDto.Mobile = data.Mobile;
            return userDto;
        }
        public async Task<bool> UpdatePassword(UserPasswordDto info, int userId)
        {
            User? user = null;
            if (userId == info.UserId || (dbContext.User.FirstOrDefault(x => x.Id == userId && x.Role == Provider.Enums.Roles.Admin)) != null)
            {
                user = dbContext.User.FirstOrDefault(x => x.Id == info.UserId && x.Password == info.OldPassword && x.IsActive == true);
                if (user == null)
                    return false;
            }
            else
                throw new InvalidUserException();
            user.UpdatedAt = DateTime.Now;
            if (!(info.NewPassword.Length >= 8 && info.NewPassword.Any(char.IsDigit) &&
           info.NewPassword.Any(char.IsLetter)
           && info.NewPassword.Any(ch => !char.IsLetterOrDigit(ch))))
            {
                throw new PasswordFormatException();
            }
            user.Password = info.NewPassword;
            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateProfile(UserInfoDto info, int userId)
        {
            
            User? user = null;
            if (userId == info.UserId || (dbContext.User.FirstOrDefault(x => x.Id == userId && x.Role == Provider.Enums.Roles.Admin)) != null)
            {
                user = dbContext.User.FirstOrDefault(x => x.Id == info.UserId && x.IsActive == true);
                if (user == null)
                    return false;
            }
            else
                throw new InvalidUserException();
            user.UpdatedAt = DateTime.Now;
            if (info.Email != null)
            {
                var email = dbContext.User.FirstOrDefault(x => x.Email == info.Email);
                if (email != null)
                    return false;
                user.Email = info.Email;
            }
            if (info.Mobile != null)
            {
                var mobile = dbContext.User.FirstOrDefault(x => x.Mobile == info.Mobile);
                if (mobile != null)
                    return false;
                user.Mobile = info.Mobile;
            }
            user.Email = user.Email;
            user.City = (info.City != null) ? info.City : user.City;
            user.Address = (info.Address != null) ? info.Address : user.Address;
            user.Mobile = user.Mobile;
            user.Name = (info.Name != null) ? info.Name : user.Name;
            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<string> GenerateOtp(string mobileNo, int userId)
        {
            var user = dbContext.User.FirstOrDefault(x => x.Id == userId && x.Mobile == mobileNo);
            if (user == null)
                return null;
            else
            {
                user.Otp = "123456";
                await dbContext.SaveChangesAsync();
                return user.Otp;
            }
        }
        public async Task<bool> ForgotPassword(string otp, string newPassword, int userId)
        {
            var user = dbContext.User.FirstOrDefault(x => x.Id == userId && x.Otp == otp);
            if (user == null) return false;
            if (!(newPassword.Length >= 8 && newPassword.Any(char.IsDigit) &&
           newPassword.Any(char.IsLetter)
           && newPassword.Any(ch => !char.IsLetterOrDigit(ch))))
            {
                throw new PasswordFormatException();
            }
            user.Password = newPassword;
            user.UpdatedAt = DateTime.Now;
            user.Otp = string.Empty;
            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<UserDto>> GetAllProfiles()
        {
            var data = dbContext.User.Where(x=>x.IsActive == true);

            if (data == null)
            return null;

            List<UserDto> users = new List<UserDto>();
            foreach(var user in data) {
                users.Add(new UserDto()
                {
                    Id = user.Id,
                    Role=user.Role,
                    Email=user.Email,
                    Address=user.Address,
                    City=user.City,
                    Name=user.Name,
                    Mobile=user.Mobile,
                });
            }
            return users;
        }
    }
}
