using Challenge_Fambec.Shared.Models.Enums;

namespace Challenge_Fambec.Shared.Models.DTOs;

/// <summary>
/// Request model for filtering products
/// </summary>
public class ProductFilterRequest
{
    /// <summary>
    /// Filter by item code (partial match)
    /// </summary>
    public string? CodItem { get; set; }
    
    /// <summary>
    /// Filter by item description (partial match)
    /// </summary>
    public string? DescrItem { get; set; }
    
    /// <summary>
    /// Filter by item types (multiple selection allowed)
    /// </summary>
    public List<TipoItem> TipoItems { get; set; } = new();
    
    /// <summary>
    /// Filter by item type (for backward compatibility)
    /// </summary>
    public TipoItem? TipoItem { get; set; }
    
    /// <summary>
    /// Filter by inventory unit
    /// </summary>
    public string? UnidInv { get; set; }
    
    /// <summary>
    /// Filter by NCM code
    /// </summary>
    public string? CodNcm { get; set; }
    
    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;
}
