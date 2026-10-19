using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.Interfaces;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _variantRepo;
        private readonly IProductRepository _productRepo; // Cần dùng để check xem Product cha có tồn tại không

        public ProductVariantService(IProductVariantRepository variantRepo, IProductRepository productRepo)
        {
            _variantRepo = variantRepo;
            _productRepo = productRepo;
        }

        public async Task<PagedResult<ProductVariantResponse>> GetVariantsAsync(BaseQueryParameters query, int? productId = null)
        {
            var (items, total) = await _variantRepo.GetAllAsync(query, productId);

            var responseItems = items.Select(v => new ProductVariantResponse
            {
                Id = v.Id,
                ProductId = v.ProductId,
                Color = v.Color,
                Size = v.Size,
                StockQuantity = v.StockQuantity ?? 0,
                Sku = v.Sku,
                ImageUrl = v.ImageUrl,
                CreatedAt = v.CreatedAt,
                ProductName = v.Product?.Name // Lấy tên sản phẩm cha (nhờ Include ở Repo)
            }).ToList();

            return new PagedResult<ProductVariantResponse>
            {
                Items = responseItems,
                TotalItems = total,
                Page = query.Page,
                Size = query.Size ?? (total > 0 ? total : 1)
            };
        }

        public async Task<ProductVariantResponse?> GetVariantByIdAsync(int id)
        {
            var v = await _variantRepo.GetByIdAsync(id);
            if (v == null) return null;

            return new ProductVariantResponse
            {
                Id = v.Id,
                ProductId = v.ProductId,
                Color = v.Color,
                Size = v.Size,
                StockQuantity = v.StockQuantity ?? 0,
                Sku = v.Sku,
                ImageUrl = v.ImageUrl,
                CreatedAt = v.CreatedAt,
                ProductName = v.Product?.Name
            };
        }

        public async Task<(bool IsSuccess, string ErrorMessage, ProductVariantResponse? Data)> CreateVariantAsync(CreateProductVariantRequest request)
        {
            // 1. Kiểm tra Product cha có tồn tại (chưa bị xóa) không
            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return (false, "Sản phẩm gốc không tồn tại hoặc đã bị xóa.", null);
            }

            // 2. Kiểm tra trùng mã SKU (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(request.Sku) && await _variantRepo.IsSkuExistsAsync(request.Sku))
            {
                return (false, "Mã SKU này đã tồn tại trong hệ thống.", null);
            }

            // 3. Mapping
            var variant = new ProductVariant
            {
                ProductId = request.ProductId,
                Color = request.Color,
                Size = request.Size,
                StockQuantity = request.StockQuantity,
                Sku = request.Sku,
                ImageUrl = request.ImageUrl,
                // Lưu giờ chuẩn VN
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
            };

            var created = await _variantRepo.CreateAsync(variant);

            return (true, string.Empty, await GetVariantByIdAsync(created.Id));
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateVariantAsync(int id, UpdateProductVariantRequest request)
        {
            var variant = await _variantRepo.GetByIdAsync(id);
            if (variant == null) return (false, "Không tìm thấy biến thể sản phẩm.");

            // Kiểm tra trùng mã SKU (Loại trừ ID hiện tại)
            if (!string.IsNullOrWhiteSpace(request.Sku) && await _variantRepo.IsSkuExistsAsync(request.Sku, id))
            {
                return (false, "Mã SKU này đã tồn tại ở một sản phẩm khác.");
            }

            variant.Color = request.Color;
            variant.Size = request.Size;
            variant.StockQuantity = request.StockQuantity;
            variant.Sku = request.Sku;
            variant.ImageUrl = request.ImageUrl;

            await _variantRepo.UpdateAsync(variant);
            return (true, string.Empty);
        }

        public async Task<bool> DeleteVariantAsync(int id)
        {
            var variant = await _variantRepo.GetByIdAsync(id);
            if (variant == null) return false;

            await _variantRepo.DeleteAsync(variant);
            return true;
        }
    }
}