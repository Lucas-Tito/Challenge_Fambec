using Microsoft.EntityFrameworkCore;
using Challenge_Fambec.Server.Data;
using Challenge_Fambec.Shared.Models.DTOs;
using Challenge_Fambec.Shared.Models.Entities;

namespace Challenge_Fambec.Server.Services
{
    /// <summary>
    /// Service for product management operations
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync(int userId, ProductFilterRequest filter)
        {
            var query = _context.Products.Where(p => p.UserId == userId);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.CodItem))
            {
                query = query.Where(p => p.CodItem.Contains(filter.CodItem));
            }

            if (!string.IsNullOrWhiteSpace(filter.DescrItem))
            {
                query = query.Where(p => p.DescrItem.Contains(filter.DescrItem));
            }

            if (filter.TipoItems.Any())
            {
                query = query.Where(p => filter.TipoItems.Contains(p.TipoItem));
            }
            else if (filter.TipoItem.HasValue)
            {
                // Backward compatibility
                query = query.Where(p => p.TipoItem == filter.TipoItem.Value);
            }

            if (filter.UnidInvs.Any())
            {
                query = query.Where(p => filter.UnidInvs.Contains(p.UnidInv));
            }
            else if (!string.IsNullOrWhiteSpace(filter.UnidInv))
            {
                // Backward compatibility
                query = query.Where(p => p.UnidInv == filter.UnidInv);
            }

            if (!string.IsNullOrWhiteSpace(filter.CodNcm))
            {
                query = query.Where(p => p.CodNcm == filter.CodNcm);
            }

            // Apply pagination
            return await query
                .OrderBy(p => p.CodItem)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int userId, int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }

        public async Task<Product?> GetProductByCodItemAsync(int userId, string codItem)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.CodItem == codItem && p.UserId == userId);
        }

        public async Task<Product> CreateProductAsync(int userId, CreateProductRequest request)
        {
            // Check if product with same CodItem already exists for this user
            if (await ExistsByCodItemAsync(userId, request.CodItem))
            {
                throw new InvalidOperationException($"Product with item code '{request.CodItem}' already exists for this user");
            }

            var product = new Product
            {
                UserId = userId,
                CodItem = request.CodItem,
                DescrItem = request.DescrItem,
                CodBarra = request.CodBarra,
                CodAntItem = request.CodAntItem,
                UnidInv = request.UnidInv,
                TipoItem = request.TipoItem,
                CodNcm = request.CodNcm,
                ExIpi = request.ExIpi,
                CodGen = request.CodGen,
                CodLst = request.CodLst,
                CodCest = request.CodCest,
                AliqIcms = request.AliqIcms,
                DataCriacao = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> UpdateProductAsync(int userId, int id, UpdateProductRequest request)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            
            if (product == null)
                return null;

            // Update fields (CodItem cannot be updated to maintain integrity)
            product.DescrItem = request.DescrItem;
            product.CodBarra = request.CodBarra;
            product.CodAntItem = request.CodAntItem;
            product.UnidInv = request.UnidInv;
            product.TipoItem = request.TipoItem;
            product.CodNcm = request.CodNcm;
            product.ExIpi = request.ExIpi;
            product.CodGen = request.CodGen;
            product.CodLst = request.CodLst;
            product.CodCest = request.CodCest;
            product.AliqIcms = request.AliqIcms;
            product.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int userId, int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByCodItemAsync(int userId, string codItem)
        {
            return await _context.Products
                .AnyAsync(p => p.CodItem == codItem && p.UserId == userId);
        }
    }
}
