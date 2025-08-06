using Challenge_Fambec.Shared.Models.Enums;

namespace Challenge_Fambec.Shared.Models
{
    /// <summary>
    /// Filter options for product filtering in the UI
    /// </summary>
    public class ProductFilterOptions
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
    /// Filter by inventory unit (for backward compatibility)
    /// </summary>
    public string? UnidInv { get; set; }
    
    /// <summary>
    /// Filter by inventory units (multiple selection allowed)
    /// </summary>
    public List<string> UnidInvs { get; set; } = new();
    
    /// <summary>
    /// Filter by NCM code
    /// </summary>
    public string? CodNcm { get; set; }        /// <summary>
        /// List of selected filter tags
        /// </summary>
        public List<string> Tags { get; set; } = new();
    }
}
