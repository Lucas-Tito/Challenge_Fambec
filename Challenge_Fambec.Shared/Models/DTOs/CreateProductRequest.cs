using System.ComponentModel.DataAnnotations;
using Challenge_Fambec.Shared.Models.Enums;

namespace Challenge_Fambec.Shared.Models.DTOs;

/// <summary>
/// Request model for creating a new product
/// </summary>
public class CreateProductRequest
{
    /// <summary>
    /// Business item code - Required and unique identifier
    /// </summary>
    [Required]
    [StringLength(60)]
    public string CodItem { get; set; } = string.Empty;
    
    /// <summary>
    /// Item description - Required field for detailed descriptions
    /// </summary>
    [Required]
    public string DescrItem { get; set; } = string.Empty;
    
    /// <summary>
    /// Barcode - Optional field
    /// </summary>
    public string? CodBarra { get; set; }
    
    /// <summary>
    /// Previous item code - Optional field with maximum 60 characters
    /// </summary>
    [StringLength(60)]
    public string? CodAntItem { get; set; }
    
    /// <summary>
    /// Inventory unit - Required field with maximum 6 characters
    /// </summary>
    [Required]
    [StringLength(6)]
    public string UnidInv { get; set; } = string.Empty;
    
    /// <summary>
    /// Item type - Required field using predefined enum choices
    /// </summary>
    [Required]
    public TipoItem TipoItem { get; set; }
    
    /// <summary>
    /// NCM code - Optional field with exactly 8 characters
    /// </summary>
    [StringLength(8)]
    public string? CodNcm { get; set; }
    
    /// <summary>
    /// IPI exception code - Optional field with exactly 3 characters
    /// </summary>
    [StringLength(3)]
    public string? ExIpi { get; set; }
    
    /// <summary>
    /// Genre code - Optional field with exactly 2 characters
    /// </summary>
    [StringLength(2)]
    public string? CodGen { get; set; }
    
    /// <summary>
    /// Service list code - Optional field with exactly 5 characters, relevant for services
    /// </summary>
    [StringLength(5)]
    public string? CodLst { get; set; }
    
    /// <summary>
    /// CEST code - Optional field with exactly 7 digits, stored as string
    /// </summary>
    [StringLength(7)]
    public string? CodCest { get; set; }
    
    /// <summary>
    /// ICMS tax rate - Optional decimal field with precision for decimal places
    /// </summary>
    public decimal? AliqIcms { get; set; }
}
