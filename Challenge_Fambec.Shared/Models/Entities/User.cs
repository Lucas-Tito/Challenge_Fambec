using System.ComponentModel.DataAnnotations;

namespace Challenge_Fambec.Shared.Models.Entities;

/// <summary>
/// Entity representing a user in the system
/// </summary>
public class User
{
    /// <summary>
    /// Primary key - Auto increment integer
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Firebase UID - Unique identifier from Firebase authentication
    /// </summary>
    [Required]
    [StringLength(128)]
    public string FirebaseUid { get; set; } = string.Empty;
    
    /// <summary>
    /// User email address from Google authentication
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name from Google profile
    /// </summary>
    [StringLength(256)]
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Profile picture URL from Google
    /// </summary>
    [StringLength(500)]
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Creation date - When user first logged in
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last login date
    /// </summary>
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property for user's products
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
