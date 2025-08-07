namespace Challenge_Fambec.Shared.Models.DTOs;

/// <summary>
/// Response model for AI-generated product summary
/// </summary>
public class ProductSummaryResponse
{
    /// <summary>
    /// The AI-generated summary content
    /// </summary>
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates if the summary generation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if summary generation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// AI model used for generation
    /// </summary>
    public string? ModelUsed { get; set; }
    
    /// <summary>
    /// Total number of products analyzed
    /// </summary>
    public int TotalProducts { get; set; }
}
