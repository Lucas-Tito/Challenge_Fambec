namespace Challenge_Fambec.Shared.Models.DTOs;

/// <summary>
/// DTO for Google authentication request
/// </summary>
public class GoogleAuthRequest
{
    /// <summary>
    /// Firebase ID token from client
    /// </summary>
    public string IdToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT token for API authentication
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// User information
    /// </summary>
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// DTO for user information
/// </summary>
public class UserDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Profile picture URL
    /// </summary>
    public string? PhotoUrl { get; set; }
}
