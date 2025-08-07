using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Challenge_Fambec.Shared.Models.DTOs;
using Challenge_Fambec.Shared.Models.Entities;
using Challenge_Fambec.Server.Services;
using System.Security.Claims;

namespace Challenge_Fambec.Server.Controllers
{
    /// <summary>
    /// Controller for product management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all product operations
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly OpenRouterService _openRouterService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService, 
            OpenRouterService openRouterService, 
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _openRouterService = openRouterService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user ID from JWT token
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userId;
        }

        /// <summary>
        /// Get paginated list of products with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts([FromQuery] ProductFilterRequest filter)
        {
            try
            {
                var userId = GetCurrentUserId();
                var products = await _productService.GetProductsAsync(userId, filter);
                return Ok(products);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products with filter: {@Filter}", filter);
                return StatusCode(500, "An error occurred while retrieving products");
            }
        }

        /// <summary>
        /// Get a specific product by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var product = await _productService.GetProductByIdAsync(userId, id);
                
                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(product);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product: {ProductId}", id);
                return StatusCode(500, "An error occurred while retrieving the product");
            }
        }

        /// <summary>
        /// Get a specific product by item code
        /// </summary>
        [HttpGet("by-code/{codItem}")]
        public async Task<ActionResult<Product>> GetProductByCodItem(string codItem)
        {
            try
            {
                var userId = GetCurrentUserId();
                var product = await _productService.GetProductByCodItemAsync(userId, codItem);
                
                if (product == null)
                    return NotFound($"Product with item code '{codItem}' not found");

                return Ok(product);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product: {CodItem}", codItem);
                return StatusCode(500, "An error occurred while retrieving the product");
            }
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var product = await _productService.CreateProductAsync(userId, request);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating product: {CodItem}", request.CodItem);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product: {CodItem}", request.CodItem);
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                var product = await _productService.UpdateProductAsync(userId, id, request);
                
                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(product);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product: {ProductId}", id);
                return StatusCode(500, "An error occurred while updating the product");
            }
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var deleted = await _productService.DeleteProductAsync(userId, id);
                
                if (!deleted)
                    return NotFound($"Product with ID {id} not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product: {ProductId}", id);
                return StatusCode(500, "An error occurred while deleting the product");
            }
        }

        /// <summary>
        /// Check if a product exists by item code
        /// </summary>
        [HttpGet("exists/{codItem}")]
        public async Task<ActionResult<bool>> ExistsByCodItem(string codItem)
        {
            try
            {
                var userId = GetCurrentUserId();
                var exists = await _productService.ExistsByCodItemAsync(userId, codItem);
                return Ok(exists);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking product existence: {CodItem}", codItem);
                return StatusCode(500, "An error occurred while checking product existence");
            }
        }

        /// <summary>
        /// Generate AI summary for all products
        /// </summary>
        [HttpPost("generate-summary")]
        public async Task<ActionResult<ProductSummaryResponse>> GenerateSummary()
        {
            try
            {
                _logger.LogInformation("Generating AI summary for user products");

                var userId = GetCurrentUserId();
                // Get all products for the current user (no filtering)
                var filter = new ProductFilterRequest { PageSize = int.MaxValue };
                var products = await _productService.GetProductsAsync(userId, filter);

                _logger.LogInformation("Retrieved {Count} products for summary for user {UserId}", products.Count, userId);

                // Generate summary using OpenRouter service
                var summaryResponse = await _openRouterService.GenerateSummaryAsync(products);

                _logger.LogInformation("OpenRouter service returned: Success={Success}, Error={Error}", 
                    summaryResponse.Success, summaryResponse.ErrorMessage);

                if (summaryResponse.Success)
                {
                    _logger.LogInformation("AI summary generated successfully using model: {Model}", 
                        summaryResponse.ModelUsed);
                    return Ok(summaryResponse);
                }
                else
                {
                    _logger.LogWarning("AI summary generation failed: {Error}", summaryResponse.ErrorMessage);
                    return BadRequest(summaryResponse);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI summary");
                return StatusCode(500, new ProductSummaryResponse
                {
                    Success = false,
                    ErrorMessage = "An error occurred while generating the summary"
                });
            }
        }
    }
}
