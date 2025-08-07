using Microsoft.AspNetCore.Mvc;
using Challenge_Fambec.Shared.Models.DTOs;
using Challenge_Fambec.Server.Services;

namespace Challenge_Fambec.Server.Controllers;

/// <summary>
/// Controller for authentication endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IFirebaseAuthService _firebaseAuthService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IFirebaseAuthService firebaseAuthService,
        ILogger<AuthController> logger)
    {
        _firebaseAuthService = firebaseAuthService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user with Google ID token
    /// </summary>
    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> AuthenticateWithGoogle([FromBody] GoogleAuthRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest("ID token is required");
            }

            var authResponse = await _firebaseAuthService.AuthenticateWithGoogleAsync(request.IdToken);
            return Ok(authResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Authentication failed: {Message}", ex.Message);
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication");
            return StatusCode(500, "Internal server error during authentication");
        }
    }

    /// <summary>
    /// Get current user information (requires authentication)
    /// </summary>
    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            var firebaseUid = User.FindFirst("firebase_uid")?.Value;
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Invalid token");
            }

            var user = await _firebaseAuthService.GetUserByFirebaseUidAsync(firebaseUid);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                PhotoUrl = user.PhotoUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, "Internal server error");
        }
    }
}
