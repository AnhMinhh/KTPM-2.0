using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using NeonGadgetStore.Web;
using NeonGadgetStore.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient - Backend API at port 5000
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000/") });

// Add Blazored LocalStorage for client-side persistence
builder.Services.AddBlazoredLocalStorage();

// Register application services
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// Initialize AuthStateService with ApiService
var authService = app.Services.GetRequiredService<AuthStateService>();
var apiService = app.Services.GetRequiredService<ApiService>();
authService.SetApiService(apiService);

await app.RunAsync();
