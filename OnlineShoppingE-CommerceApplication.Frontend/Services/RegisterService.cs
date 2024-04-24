using OnlineShoppingECommerceApplication.Frontend.Interfaces;
using OnlineShoppingECommerceApplication.Frontend.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace OnlineShoppingECommerceApplication.Frontend.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly HttpClient httpClient;
        public RegisterService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> Register(UserDto user)
        {
            var response = await httpClient.PostAsJsonAsync<UserDto>("api/User/PostCustomer", user);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            else
                return true;
        }
    }
}
