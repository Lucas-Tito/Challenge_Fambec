using Challenge_Fambec.Shared.Models.Entities;
using Challenge_Fambec.Shared.Models.DTOs;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Challenge_Fambec.Client.Services;

/// <summary>
/// Service for handling product-related API operations
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets a list of products with optional filtering
    /// </summary>
    Task<List<Product>> GetProductsAsync(ProductFilterRequest? filter = null);
    
    /// <summary>
    /// Gets a specific product by ID
    /// </summary>
    Task<Product?> GetProductByIdAsync(int id);
    
    /// <summary>
    /// Creates a new product
    /// </summary>
    Task<Product?> CreateProductAsync(CreateProductRequest request);
    
    /// <summary>
    /// Updates an existing product
    /// </summary>
    Task<Product?> UpdateProductAsync(int id, UpdateProductRequest request);
    
    /// <summary>
    /// Deletes a product
    /// </summary>
    Task<bool> DeleteProductAsync(int id);
    
    /// <summary>
    /// Checks if a product exists by item code
    /// </summary>
    Task<bool> ExistsByCodItemAsync(string codItem);
}

/// <summary>
/// Implementation of product service using HttpClient
/// </summary>
public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;
    private const string BaseUrl = "api/products";

    public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Product>> GetProductsAsync(ProductFilterRequest? filter = null)
    {
        try
        {
            var url = BuildFilterUrl(filter);
            _logger.LogInformation("Requesting products with URL: {Url}", url);
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                return products ?? new List<Product>();
            }
            
            _logger.LogWarning("Failed to get products. Status: {StatusCode}", response.StatusCode);
            return new List<Product>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return new List<Product>();
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            
            if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Failed to get product {ProductId}. Status: {StatusCode}", id, response.StatusCode);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return null;
        }
    }

    public async Task<Product?> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            
            _logger.LogWarning("Failed to create product. Status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return null;
        }
    }

    public async Task<Product?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            
            _logger.LogWarning("Failed to update product {ProductId}. Status: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return false;
        }
    }

    public async Task<bool> ExistsByCodItemAsync(string codItem)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/exists/{Uri.EscapeDataString(codItem)}");
            
            if (response.IsSuccessStatusCode)
            {
                var exists = await response.Content.ReadFromJsonAsync<bool>();
                return exists;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if product exists {CodItem}", codItem);
            return false;
        }
    }

    /// <summary>
    /// Builds the URL with query parameters for filtering
    /// </summary>
    private string BuildFilterUrl(ProductFilterRequest? filter)
    {
        if (filter == null)
        {
            return BaseUrl;
        }

        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(filter.CodItem))
            queryParams.Add($"CodItem={Uri.EscapeDataString(filter.CodItem)}");

        if (!string.IsNullOrEmpty(filter.DescrItem))
            queryParams.Add($"DescrItem={Uri.EscapeDataString(filter.DescrItem)}");

        if (filter.TipoItems.Any())
        {
            foreach (var tipoItem in filter.TipoItems)
            {
                queryParams.Add($"TipoItems={tipoItem}");
            }
        }
        else if (filter.TipoItem.HasValue)
        {
            // Backward compatibility
            queryParams.Add($"TipoItem={filter.TipoItem}");
        }

        if (!string.IsNullOrEmpty(filter.UnidInv))
            queryParams.Add($"UnidInv={Uri.EscapeDataString(filter.UnidInv)}");

        if (!string.IsNullOrEmpty(filter.CodNcm))
            queryParams.Add($"CodNcm={Uri.EscapeDataString(filter.CodNcm)}");

        queryParams.Add($"Page={filter.Page}");
        queryParams.Add($"PageSize={filter.PageSize}");

        return queryParams.Count > 0 ? $"{BaseUrl}?{string.Join("&", queryParams)}" : BaseUrl;
    }
}
