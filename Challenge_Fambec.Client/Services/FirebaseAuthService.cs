using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using Challenge_Fambec.Shared.Models.DTOs;

namespace Challenge_Fambec.Client.Services;

/// <summary>
/// Interface for Firebase authentication service on client-side
/// </summary>
public interface IFirebaseAuthService
{
    Task<AuthResponse?> SignInWithGoogleAsync();
    Task SignOutAsync();
    Task<UserDto?> GetCurrentUserAsync();
    bool IsAuthenticated { get; }
    event Action<UserDto?> AuthStateChanged;
}

/// <summary>
/// Client-side Firebase authentication service
/// </summary>
public class FirebaseAuthService : IFirebaseAuthService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private UserDto? _currentUser;
    private string? _currentToken;

    public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_currentToken);
    public event Action<UserDto?>? AuthStateChanged;

    public FirebaseAuthService(
        HttpClient httpClient, 
        IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Sign in with Google using Firebase
    /// </summary>
    public async Task<AuthResponse?> SignInWithGoogleAsync()
    {
        try
        {
            // Call JavaScript function to handle Google sign-in
            var idToken = await _jsRuntime.InvokeAsync<string>("firebaseAuth.signInWithGoogle");
            
            if (string.IsNullOrEmpty(idToken))
            {
                return null;
            }

            // Send token to backend for verification
            var response = await _httpClient.PostAsJsonAsync("api/auth/google", 
                new GoogleAuthRequest { IdToken = idToken });

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (authResponse != null)
                {
                    _currentUser = authResponse.User;
                    _currentToken = authResponse.Token;
                    
                    // Set Authorization header for future requests
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);

                    // Store token in localStorage
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_token", _currentToken);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user_info", JsonSerializer.Serialize(_currentUser));

                    AuthStateChanged?.Invoke(_currentUser);
                    
                    return authResponse;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during Google sign-in: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Sign out user
    /// </summary>
    public async Task SignOutAsync()
    {
        try
        {
            // Sign out from Firebase
            await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
            
            // Clear local state
            _currentUser = null;
            _currentToken = null;
            
            // Clear authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            // Clear localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user_info");
            
            AuthStateChanged?.Invoke(null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during sign out: {ex.Message}");
        }
    }

    /// <summary>
    /// Get current authenticated user
    /// </summary>
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        try
        {
            // Try to restore from localStorage
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_token");
            var userInfo = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user_info");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userInfo))
            {
                _currentToken = token;
                _currentUser = JsonSerializer.Deserialize<UserDto>(userInfo, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (_currentUser != null)
                {
                    // Set authorization header
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _currentToken);

                    // Verify token with backend
                    try
                    {
                        var response = await _httpClient.GetAsync("api/auth/me");
                        if (response.IsSuccessStatusCode)
                        {
                            return _currentUser;
                        }
                        else
                        {
                            // Token is invalid, clear it
                            await SignOutAsync();
                        }
                    }
                    catch
                    {
                        // Network error or server down, keep local session
                        return _currentUser;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting current user: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Initialize authentication state on app startup
    /// </summary>
    public async Task InitializeAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user != null)
        {
            AuthStateChanged?.Invoke(user);
        }
    }

    public void Dispose()
    {
        // Clean up event handlers if needed
    }
}
