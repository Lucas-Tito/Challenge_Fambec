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
        /// Filter by item type
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
        /// List of selected filter tags
        /// </summary>
        public List<string> Tags { get; set; } = new();
    }
}
