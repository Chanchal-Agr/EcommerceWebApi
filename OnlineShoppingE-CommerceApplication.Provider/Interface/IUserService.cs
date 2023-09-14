using OnlineShoppingE_CommerceApplication.Provider.Enums;
using OnlineShoppingE_CommerceApplication.Provider.Entities;
using OnlineShoppingE_CommerceApplication.Provider.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.Interface
{
    public interface IUserService
    {
        Task<int> Post(User user);
        Task<UserDto> Profile(int id,int userId);
        Task<bool> UpdatePassword(UserPasswordDto info,int  userId);
        Task<bool> UpdateProfile(UserInfoDto info, int userId);
        Task<string> GenerateOtp(string mobileNo, int userId);
        Task<bool> ForgotPassword(string otp, string newPassword,int userId);

    }
}
