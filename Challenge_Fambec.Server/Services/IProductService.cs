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
        /// Get paginated list of products with optional filtering for a specific user
        /// </summary>
        /// <param name="userId">User ID to filter products</param>
        /// <param name="filter">Filter criteria for products</param>
        /// <returns>List of products matching the filter criteria for the user</returns>
        Task<List<Product>> GetProductsAsync(int userId, ProductFilterRequest filter);
        
        /// <summary>
        /// Get a specific product by its ID for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="id">Product ID</param>
        /// <returns>Product if found and belongs to user, null otherwise</returns>
        Task<Product?> GetProductByIdAsync(int userId, int id);
        
        /// <summary>
        /// Get a specific product by its item code for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="codItem">Item code</param>
        /// <returns>Product if found and belongs to user, null otherwise</returns>
        Task<Product?> GetProductByCodItemAsync(int userId, string codItem);
        
        /// <summary>
        /// Create a new product for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Product creation request</param>
        /// <returns>Created product</returns>
        Task<Product> CreateProductAsync(int userId, CreateProductRequest request);
        
        /// <summary>
        /// Update an existing product for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="id">Product ID to update</param>
        /// <param name="request">Product update request</param>
        /// <returns>Updated product if found and belongs to user, null otherwise</returns>
        Task<Product?> UpdateProductAsync(int userId, int id, UpdateProductRequest request);
        
        /// <summary>
        /// Delete a product for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="id">Product ID to delete</param>
        /// <returns>True if deleted successfully, false if not found or doesn't belong to user</returns>
        Task<bool> DeleteProductAsync(int userId, int id);
        
        /// <summary>
        /// Check if a product with the given item code already exists for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="codItem">Item code to check</param>
        /// <returns>True if exists for the user, false otherwise</returns>
        Task<bool> ExistsByCodItemAsync(int userId, string codItem);
    }
}
