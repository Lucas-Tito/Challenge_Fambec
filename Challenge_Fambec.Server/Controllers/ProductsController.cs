using Microsoft.AspNetCore.Mvc;
using Challenge_Fambec.Shared.Models.DTOs;
using Challenge_Fambec.Shared.Models.Entities;
using Challenge_Fambec.Server.Services;

namespace Challenge_Fambec.Server.Controllers
{
    /// <summary>
    /// Controller for product management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of products with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts([FromQuery] ProductFilterRequest filter)
        {
            try
            {
                var products = await _productService.GetProductsAsync(filter);
                return Ok(products);
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
                var product = await _productService.GetProductByIdAsync(id);
                
                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(product);
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
                var product = await _productService.GetProductByCodItemAsync(codItem);
                
                if (product == null)
                    return NotFound($"Product with item code '{codItem}' not found");

                return Ok(product);
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

                var product = await _productService.CreateProductAsync(request);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
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

                var product = await _productService.UpdateProductAsync(id, request);
                
                if (product == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(product);
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
                var deleted = await _productService.DeleteProductAsync(id);
                
                if (!deleted)
                    return NotFound($"Product with ID {id} not found");

                return NoContent();
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
                var exists = await _productService.ExistsByCodItemAsync(codItem);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking product existence: {CodItem}", codItem);
                return StatusCode(500, "An error occurred while checking product existence");
            }
        }
    }
}
