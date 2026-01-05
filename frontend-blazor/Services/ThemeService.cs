using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace NeonGadgetStore.Web.Services;

public class ThemeService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;
    private const string ThemeKey = "theme";
    
    public bool IsDarkMode { get; private set; }
    
    public event Action? OnChange;

    public ThemeService(ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
        _localStorage = localStorage;
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var theme = await _localStorage.GetItemAsync<string>(ThemeKey);
            IsDarkMode = theme == "dark";
            await ApplyThemeAsync();
        }
        catch
        {
            IsDarkMode = false;
        }
    }

    public async Task ToggleThemeAsync()
    {
        IsDarkMode = !IsDarkMode;
        await _localStorage.SetItemAsync(ThemeKey, IsDarkMode ? "dark" : "light");
        await ApplyThemeAsync();
        NotifyStateChanged();
    }

    public async Task SetDarkModeAsync(bool isDark)
    {
        IsDarkMode = isDark;
        await _localStorage.SetItemAsync(ThemeKey, IsDarkMode ? "dark" : "light");
        await ApplyThemeAsync();
        NotifyStateChanged();
    }

    private async Task ApplyThemeAsync()
    {
        try
        {
            if (IsDarkMode)
            {
                await _jsRuntime.InvokeVoidAsync("eval", "document.documentElement.classList.add('dark')");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("eval", "document.documentElement.classList.remove('dark')");
            }
        }
        catch
        {
            // Ignore during prerendering
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
