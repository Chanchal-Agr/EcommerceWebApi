using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingE_CommerceApplication.Provider.DTOs
{
    public class LoginResponse
    {
        public UserDto UserInfo { get; set; }
        public string Token { get; set; }
    }
}
