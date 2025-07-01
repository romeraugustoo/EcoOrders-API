using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrdenesAPI.Data;
using OrdenesAPI.DTOs;
using OrdenesAPI.Services.Interfaces;

namespace OrdenesAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == request.SKU))
                throw new ArgumentException("El SKU ya está en uso por otro producto.");

            if (await _context.Products.AnyAsync(p => p.InternalCode == request.InternalCode))
                throw new ArgumentException("El Código Interno ya está en uso por otro producto.");

            var product = new Models.Product
            {
                ProductId = Guid.NewGuid(),
                SKU = request.SKU,
                InternalCode = request.InternalCode,
                Name = request.Name,
                Description = request.Description,
                CurrentUnitPrice = request.CurrentUnitPrice,
                StockQuantity = request.StockQuantity,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductResponse
            {
                ProductId = product.ProductId,
                SKU = product.SKU,
                InternalCode = product.InternalCode,
                Name = product.Name,
                Description = product.Description,
                CurrentUnitPrice = product.CurrentUnitPrice,
                StockQuantity = product.StockQuantity,
            };
        }

        public async Task<ProductResponse?> GetProductByIdAsync(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return null;

            return new ProductResponse
            {
                ProductId = product.ProductId,
                SKU = product.SKU,
                InternalCode = product.InternalCode,
                Name = product.Name,
                Description = product.Description,
                CurrentUnitPrice = product.CurrentUnitPrice,
                StockQuantity = product.StockQuantity,
            };
        }

        public async Task<PaginatedResponse<ProductResponse>> GetProductsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null
        )
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentException("pageNumber y pageSize deben ser mayores que cero.");

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm)
                    || (p.Description != null && p.Description.Contains(searchTerm))
                );
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.CurrentUnitPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.CurrentUnitPrice <= maxPrice.Value);
            }
            var totalItems = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = products
                .Select(p => new ProductResponse
                {
                    ProductId = p.ProductId,
                    SKU = p.SKU,
                    InternalCode = p.InternalCode,
                    Name = p.Name,
                    Description = p.Description,
                    CurrentUnitPrice = p.CurrentUnitPrice,
                    StockQuantity = p.StockQuantity,
                })
                .ToList();

            return new PaginatedResponse<ProductResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = items,
            };
        }

        public async Task<ProductResponse> UpdateProductAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("Producto no encontrado.");
            }

            // Actualizar campos opcionales
            if (request.Description != null)
            {
                product.Description = request.Description;
            }

            if (request.CurrentUnitPrice.HasValue)
            {
                product.CurrentUnitPrice = request.CurrentUnitPrice.Value;
            }

            if (request.StockQuantity.HasValue)
            {
                product.StockQuantity = request.StockQuantity.Value;
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return new ProductResponse
            {
                ProductId = product.ProductId,
                SKU = product.SKU,
                InternalCode = product.InternalCode,
                Name = product.Name,
                Description = product.Description,
                CurrentUnitPrice = product.CurrentUnitPrice,
                StockQuantity = product.StockQuantity,
            };
        }
    }
}
