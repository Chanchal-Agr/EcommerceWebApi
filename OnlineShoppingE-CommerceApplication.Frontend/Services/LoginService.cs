using OnlineShoppingECommerceApplication.Frontend.Interfaces;
using OnlineShoppingECommerceApplication.Frontend.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace OnlineShoppingECommerceApplication.Frontend.Services
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient httpClient;
        public LoginService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<LoginResponse> Authenticate(LoginRequest login)
        {
            var response = await httpClient.PostAsJsonAsync<LoginRequest>("api/Login", login);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(content);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);
            return loginResponse;
        }
    }
}
