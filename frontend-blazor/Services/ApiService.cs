using System.Net.Http.Json;
using System.Text.Json;
using NeonGadgetStore.Web.Models;

namespace NeonGadgetStore.Web.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;
    // Backend API URL
    private const string BaseUrl = "http://localhost:5000/api";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public ApiService(HttpClient httpClient, AuthStateService authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    private void SetAuthHeader()
    {
        var token = _authState.Token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    // Products
    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/products");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Product>>(json, JsonOptions) ?? new List<Product>();
            }
            return new List<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching products: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<Product?> GetProductAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Product>(json, JsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching product: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string categoryId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/products?categoryId={categoryId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Product>>(json, JsonOptions) ?? new List<Product>();
            }
            return new List<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching products by category: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<List<Product>> SearchProductsAsync(string query)
    {
        try
        {
            // Get all products and filter client-side (backend doesn't have search endpoint)
            var products = await GetProductsAsync();
            var lowerQuery = query.ToLower();
            return products.Where(p => 
                p.Name.ToLower().Contains(lowerQuery) || 
                (p.Description?.ToLower().Contains(lowerQuery) ?? false))
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching products: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<List<Product>> GetFeaturedProductsAsync()
    {
        try
        {
            var products = await GetProductsAsync();
            return products.Where(p => p.IsFeatured).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching featured products: {ex.Message}");
            return new List<Product>();
        }
    }

    // Categories
    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/categories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Category>>(json, JsonOptions) ?? new List<Category>();
            }
            return new List<Category>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching categories: {ex.Message}");
            return new List<Category>();
        }
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Category>(json, JsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching category: {ex.Message}");
            return null;
        }
    }

    // Auth
    public async Task<(AuthResponse? Data, string? Error)> SignUpAsync(string email, string password, string username, string fullName)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/signup", new
            {
                email,
                password,
                username,
                full_name = fullName
            });

            var json = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<AuthResponse>(json, JsonOptions);
                return (data, null);
            }
            
            return (null, json);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }

    public async Task<(AuthResponse? Data, string? Error)> SignInAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/signin", new
            {
                email,
                password
            });

            var json = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<AuthResponse>(json, JsonOptions);
                return (data, null);
            }
            
            return (null, json);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }

    public async Task<MeResponse?> GetMeAsync()
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/me");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MeResponse>(json, JsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user: {ex.Message}");
            return null;
        }
    }

    public async Task<(Profile? Data, string? Error)> UpdateProfileAsync(Profile profile)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/me/profile", profile);
            var json = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<Profile>(json, JsonOptions);
                return (data, null);
            }
            
            return (null, json);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }

    // Orders
    public async Task<List<Order>> GetOrdersAsync()
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/orders");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Order>>(json, JsonOptions) ?? new List<Order>();
            }
            return new List<Order>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching orders: {ex.Message}");
            return new List<Order>();
        }
    }

    public async Task<Order?> GetOrderAsync(string id)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/orders/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Order>(json, JsonOptions);
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching order: {ex.Message}");
            return null;
        }
    }

    public async Task<(Order? Data, string? Error)> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/orders", request);
            var json = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<Order>(json, JsonOptions);
                return (data, null);
            }
            
            return (null, json);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }
    }

    // Wishlist
    public async Task<List<Product>> GetWishlistAsync()
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/wishlist");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var entries = JsonSerializer.Deserialize<List<WishlistEntry>>(json, JsonOptions) ?? new List<WishlistEntry>();
                // Extract products from wishlist entries
                return entries.Where(e => e.Product != null).Select(e => e.Product!).ToList();
            }
            return new List<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching wishlist: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<bool> AddToWishlistAsync(string productId)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/wishlist", new { product_id = productId });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveFromWishlistAsync(string productId)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/wishlist/{productId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // Recently Viewed
    public async Task<List<Product>> GetRecentlyViewedAsync()
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"{BaseUrl}/viewed");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var entries = JsonSerializer.Deserialize<List<ViewedEntry>>(json, JsonOptions) ?? new List<ViewedEntry>();
                // Extract products from viewed entries
                return entries.Where(e => e.Product != null).Select(e => e.Product!).ToList();
            }
            return new List<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching recently viewed: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<bool> AddToViewedAsync(string productId)
    {
        try
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/viewed", new { product_id = productId });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
