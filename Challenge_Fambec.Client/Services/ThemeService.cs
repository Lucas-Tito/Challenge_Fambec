using Microsoft.JSInterop;

namespace Challenge_Fambec.Client.Services;

public interface IThemeService
{
    event Action? OnThemeChanged;
    Task<bool> GetIsDarkModeAsync();
    Task ToggleThemeAsync();
    Task InitializeThemeAsync();
}

public class ThemeService : IThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode;

    public event Action? OnThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> GetIsDarkModeAsync()
    {
        try
        {
            _isDarkMode = await _jsRuntime.InvokeAsync<bool>("themeManager.getCurrentTheme");
            return _isDarkMode;
        }
        catch
        {
            return _isDarkMode;
        }
    }

    public async Task ToggleThemeAsync()
    {
        try
        {
            _isDarkMode = await _jsRuntime.InvokeAsync<bool>("themeManager.toggleTheme");
            OnThemeChanged?.Invoke();
        }
        catch
        {
            // Se falhar, use o fallback local
            _isDarkMode = !_isDarkMode;
            OnThemeChanged?.Invoke();
        }
    }

    public async Task InitializeThemeAsync()
    {
        try
        {
            _isDarkMode = await _jsRuntime.InvokeAsync<bool>("themeManager.initTheme");
        }
        catch
        {
            // Fallback para tema claro se houver erro
            _isDarkMode = false;
        }
    }
}
