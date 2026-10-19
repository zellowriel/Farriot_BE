using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponse>> GetProductsAsync(BaseQueryParameters query);
        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<bool> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);
    }
}