using Newtonsoft.Json;

namespace OnlineShoppingECommerceApplication.Frontend.Models
{
    public class LoginResponse
    {
        [JsonProperty(PropertyName = "userInfo")]
        public UserDto UserInfo { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
