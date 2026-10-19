using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface IProductVariantService
    {
        Task<PagedResult<ProductVariantResponse>> GetVariantsAsync(BaseQueryParameters query, int? productId = null);
        Task<ProductVariantResponse?> GetVariantByIdAsync(int id);
        Task<(bool IsSuccess, string ErrorMessage, ProductVariantResponse? Data)> CreateVariantAsync(CreateProductVariantRequest request);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateVariantAsync(int id, UpdateProductVariantRequest request);
        Task<bool> DeleteVariantAsync(int id);
    }
}