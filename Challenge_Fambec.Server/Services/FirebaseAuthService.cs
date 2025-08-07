using Challenge_Fambec.Shared.Models.Entities;
using Challenge_Fambec.Shared.Models.DTOs;
using Challenge_Fambec.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace Challenge_Fambec.Server.Services;

/// <summary>
/// Interface for Firebase authentication service
/// </summary>
public interface IFirebaseAuthService
{
    Task<AuthResponse> AuthenticateWithGoogleAsync(string idToken);
    Task<User?> GetUserByFirebaseUidAsync(string firebaseUid);
    string GenerateJwtToken(User user);
}

/// <summary>
/// Service for handling Firebase authentication without Admin SDK
/// </summary>
public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebaseAuthService> _logger;
    private readonly HttpClient _httpClient;

    public FirebaseAuthService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<FirebaseAuthService> logger,
        HttpClient httpClient)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Authenticate user with Google ID token
    /// </summary>
    public async Task<AuthResponse> AuthenticateWithGoogleAsync(string idToken)
    {
        try
        {
            _logger.LogInformation("Starting Firebase token verification");

            // For development, we'll extract user info directly from the ID token
            // In production, you should verify the token signature properly
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(idToken);

            var firebaseUid = jsonToken.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value 
                            ?? jsonToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            var email = jsonToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "";
            var name = jsonToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
            var picture = jsonToken.Claims.FirstOrDefault(x => x.Type == "picture")?.Value;

            if (string.IsNullOrEmpty(firebaseUid))
            {
                _logger.LogWarning("No Firebase UID found in token");
                throw new UnauthorizedAccessException("Invalid token format");
            }

            _logger.LogInformation("Token parsed successfully for user: {Email}", email);

            // Find or create user in database
            var user = await GetOrCreateUserAsync(firebaseUid, email, name, picture);

            // Generate JWT token for API authentication
            var jwtToken = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = jwtToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    PhotoUrl = user.PhotoUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user with Firebase");
            throw new UnauthorizedAccessException($"Invalid authentication token: {ex.Message}");
        }
    }

    /// <summary>
    /// Get or create user in database
    /// </summary>
    private async Task<User> GetOrCreateUserAsync(string firebaseUid, string email, string? displayName, string? photoUrl)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

        if (user == null)
        {
            // Create new user
            user = new User
            {
                FirebaseUid = firebaseUid,
                Email = email,
                DisplayName = displayName,
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new user: {Email}", email);
        }
        else
        {
            // Update last login time and profile info if changed
            user.LastLoginAt = DateTime.UtcNow;
            if (user.DisplayName != displayName) user.DisplayName = displayName;
            if (user.PhotoUrl != photoUrl) user.PhotoUrl = photoUrl;
            
            await _context.SaveChangesAsync();
        }

        return user;
    }

    /// <summary>
    /// Get user by Firebase UID
    /// </summary>
    public async Task<User?> GetUserByFirebaseUidAsync(string firebaseUid)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
    }

    /// <summary>
    /// Generate JWT token for API authentication
    /// </summary>
    public string GenerateJwtToken(User user)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "your-secret-key-minimum-32-characters-long";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("firebase_uid", user.FirebaseUid),
                new Claim("display_name", user.DisplayName ?? "")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

/// <summary>
/// Firebase API response models
/// </summary>
public class FirebaseVerifyResponse
{
    public FirebaseUser[]? users { get; set; }
}

public class FirebaseUser
{
    public string localId { get; set; } = "";
    public string? email { get; set; }
    public string? displayName { get; set; }
    public string? photoUrl { get; set; }
    public bool emailVerified { get; set; }
}
