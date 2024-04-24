using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace OnlineShoppingECommerceApplication.Frontend.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localstorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            localstorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localstorage.GetItemAsync<string>("Token");
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            else
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Role, "admin")
                };
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await localstorage.SetItemAsync("Token", token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            await localstorage.RemoveItemAsync(token);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await localstorage.RemoveItemAsync("Token");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
