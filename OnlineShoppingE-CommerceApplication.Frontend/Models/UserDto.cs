using Newtonsoft.Json;
using OnlineShoppingECommerceApplication.Frontend.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingECommerceApplication.Frontend.Models
{
    public class UserDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "mobile")]
        public string Mobile { get; set; }
        [JsonProperty(PropertyName = "email")]

        public string? Email { get; set; }
        [JsonProperty(PropertyName = "address")]

        public string? Address { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string? City { get; set; }

        [JsonProperty(PropertyName = "role")]
        public Roles Role { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}