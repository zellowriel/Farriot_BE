using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;

namespace EXE_Farriott_BE.Repositories.Interfaces
{
    public interface IProductVariantRepository
    {
        Task<(List<ProductVariant> items, int total)> GetAllAsync(BaseQueryParameters query, int? productId = null);
        Task<ProductVariant?> GetByIdAsync(int id);
        Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null); // Hàm kiểm tra trùng SKU
        Task<ProductVariant> CreateAsync(ProductVariant variant);
        Task UpdateAsync(ProductVariant variant);
        Task DeleteAsync(ProductVariant variant);
    }
}