using Blazored.LocalStorage;
using NeonGadgetStore.Web.Models;

namespace NeonGadgetStore.Web.Services;

public class AuthStateService
{
    private readonly ILocalStorageService _localStorage;
    private ApiService? _apiService;
    private const string TokenKey = "auth_token";
    private const string UserKey = "current_user";
    
    public UserWithProfile? CurrentUser { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    public string? Token { get; private set; }
    public bool IsLoading { get; private set; } = true;
    public string? LastError { get; private set; }

    public event Action? OnChange;

    public AuthStateService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public void SetApiService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;
        NotifyStateChanged();

        try
        {
            Token = await _localStorage.GetItemAsync<string>(TokenKey);
            
            if (!string.IsNullOrEmpty(Token) && _apiService != null)
            {
                var meResponse = await _apiService.GetMeAsync();
                if (meResponse != null)
                {
                    CurrentUser = new UserWithProfile
                    {
                        Id = meResponse.User.Id,
                        Email = meResponse.User.Email,
                        Profile = meResponse.Profile,
                        Roles = meResponse.Roles ?? new List<string>()
                    };
                    IsAdmin = meResponse.Roles?.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ?? false;
                }
                else
                {
                    await ClearAsync();
                }
            }
        }
        catch
        {
            await ClearAsync();
        }

        IsLoading = false;
        NotifyStateChanged();
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        if (_apiService == null) return false;

        try
        {
            var (response, error) = await _apiService.SignInAsync(email, password);
            
            if (response != null)
            {
                Token = response.Token;
                CurrentUser = new UserWithProfile
                {
                    Id = response.User.Id,
                    Email = response.User.Email,
                    Profile = response.Profile,
                    Roles = response.Roles ?? new List<string>()
                };
                IsAdmin = response.Roles?.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ?? false;
                
                await _localStorage.SetItemAsync(TokenKey, Token);
                NotifyStateChanged();
                return true;
            }
            
            Console.WriteLine($"Login error: {error}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login exception: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RegisterAsync(string email, string password, string fullName)
    {
        if (_apiService == null) return false;

        try
        {
            // Generate username from email
            var username = email.Split('@')[0].ToLower().Replace(".", "_");
            
            var (response, error) = await _apiService.SignUpAsync(email, password, username, fullName);
            
            if (response != null)
            {
                Token = response.Token;
                CurrentUser = new UserWithProfile
                {
                    Id = response.User.Id,
                    Email = response.User.Email,
                    Profile = response.Profile,
                    Roles = response.Roles ?? new List<string>()
                };
                IsAdmin = response.Roles?.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)) ?? false;
                
                await _localStorage.SetItemAsync(TokenKey, Token);
                LastError = null;
                NotifyStateChanged();
                return true;
            }
            
            LastError = error;
            Console.WriteLine($"Register error: {error}");
            return false;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            Console.WriteLine($"Register exception: {ex.Message}");
            return false;
        }
    }

    public async Task SetAuthAsync(AuthResponse response)
    {
        Token = response.Token;
        CurrentUser = new UserWithProfile
        {
            Id = response.User.Id,
            Email = response.User.Email,
            Profile = response.Profile,
            Roles = response.Roles ?? new List<string>()
        };
        IsAdmin = response.Roles?.Contains("admin") ?? false;
        
        await _localStorage.SetItemAsync(TokenKey, Token);
        NotifyStateChanged();
    }

    public void UpdateProfile(Profile profile)
    {
        if (CurrentUser != null)
        {
            CurrentUser.Profile = profile;
            NotifyStateChanged();
        }
    }

    public async Task LogoutAsync()
    {
        await ClearAsync();
    }

    public async Task ClearAsync()
    {
        Token = null;
        CurrentUser = null;
        IsAdmin = false;
        
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UserKey);
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
