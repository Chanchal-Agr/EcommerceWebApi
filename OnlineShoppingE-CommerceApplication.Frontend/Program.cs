using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlineShoppingE_CommerceApplication.Frontend;
using OnlineShoppingECommerceApplication.Frontend.Interfaces;
using OnlineShoppingECommerceApplication.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7035/") });
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
