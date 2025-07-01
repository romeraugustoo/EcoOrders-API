using System;
using System.Threading.Tasks;
using OrdenesAPI.DTOs;

namespace OrdenesAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<ProductResponse?> GetProductByIdAsync(Guid productId);
        Task<PaginatedResponse<ProductResponse>> GetProductsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null
        );
        Task<ProductResponse> UpdateProductAsync(Guid id, UpdateProductRequest request);
    }
}
