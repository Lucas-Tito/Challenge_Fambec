using Challenge_Fambec.Shared.Models.DTOs;
using Challenge_Fambec.Shared.Models.Entities;

namespace Challenge_Fambec.Server.Services
{
    /// <summary>
    /// Interface for product management operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Get paginated list of products with optional filtering
        /// </summary>
        /// <param name="filter">Filter criteria for products</param>
        /// <returns>List of products matching the filter criteria</returns>
        Task<List<Product>> GetProductsAsync(ProductFilterRequest filter);
        
        /// <summary>
        /// Get a specific product by its ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product if found, null otherwise</returns>
        Task<Product?> GetProductByIdAsync(int id);
        
        /// <summary>
        /// Get a specific product by its item code
        /// </summary>
        /// <param name="codItem">Item code</param>
        /// <returns>Product if found, null otherwise</returns>
        Task<Product?> GetProductByCodItemAsync(string codItem);
        
        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="request">Product creation request</param>
        /// <returns>Created product</returns>
        Task<Product> CreateProductAsync(CreateProductRequest request);
        
        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID to update</param>
        /// <param name="request">Product update request</param>
        /// <returns>Updated product if found, null otherwise</returns>
        Task<Product?> UpdateProductAsync(int id, UpdateProductRequest request);
        
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        Task<bool> DeleteProductAsync(int id);
        
        /// <summary>
        /// Check if a product with the given item code already exists
        /// </summary>
        /// <param name="codItem">Item code to check</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsByCodItemAsync(string codItem);
    }
}
