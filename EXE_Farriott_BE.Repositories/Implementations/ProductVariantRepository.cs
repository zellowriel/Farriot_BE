using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EXE_Farriott_BE.Repositories.Implementations
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly AppDbContext _context;

        public ProductVariantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ProductVariant> items, int total)> GetAllAsync(BaseQueryParameters query, int? productId = null)
        {
            // 1. Lọc bỏ các variant đã bị xóa mềm
            var dbQuery = _context.ProductVariants
                .Include(v => v.Product) // Kèm theo thông tin Product cha
                .Where(v => !v.IsDeleted)
                .AsQueryable();

            // Lọc theo ProductId nếu có truyền vào
            if (productId.HasValue && productId.Value > 0)
            {
                dbQuery = dbQuery.Where(v => v.ProductId == productId.Value);
            }

            // 2. Tự động tìm kiếm động (Search)
            dbQuery = dbQuery.ApplyDynamicSearch(query.SearchBy, query.Keyword);

            // 3. Sắp xếp (Sort)
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                bool isDesc = query.SortBy.StartsWith("-");
                string sortCol = isDesc ? query.SortBy.Substring(1).ToLower() : query.SortBy.ToLower();

                dbQuery = sortCol switch
                {
                    "stock" or "stockquantity" => isDesc ? dbQuery.OrderByDescending(v => v.StockQuantity) : dbQuery.OrderBy(v => v.StockQuantity),
                    "color" => isDesc ? dbQuery.OrderByDescending(v => v.Color) : dbQuery.OrderBy(v => v.Color),
                    "size" => isDesc ? dbQuery.OrderByDescending(v => v.Size) : dbQuery.OrderBy(v => v.Size),
                    "createdat" => isDesc ? dbQuery.OrderByDescending(v => v.CreatedAt) : dbQuery.OrderBy(v => v.CreatedAt),
                    _ => isDesc ? dbQuery.OrderByDescending(v => v.Id) : dbQuery.OrderBy(v => v.Id)
                };
            }
            else
            {
                dbQuery = dbQuery.OrderByDescending(v => v.CreatedAt);
            }

            // Đếm tổng
            int total = await dbQuery.CountAsync();

            // 4. Phân trang (Paging)
            List<ProductVariant> items;
            if (query.Size.HasValue && query.Size.Value > 0)
            {
                items = await dbQuery
                    .Skip((query.Page - 1) * query.Size.Value)
                    .Take(query.Size.Value)
                    .ToListAsync();
            }
            else
            {
                items = await dbQuery.ToListAsync();
            }

            return (items, total);
        }

        public async Task<ProductVariant?> GetByIdAsync(int id)
        {
            // Chặn lấy biến thể đã xóa mềm
            return await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);
        }

        public async Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(sku)) return false;

            var query = _context.ProductVariants.Where(v => v.Sku == sku && !v.IsDeleted);
            if (excludeId.HasValue)
            {
                query = query.Where(v => v.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<ProductVariant> CreateAsync(ProductVariant variant)
        {
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();
            return variant;
        }

        public async Task UpdateAsync(ProductVariant variant)
        {
            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductVariant variant)
        {
            // Xóa mềm chuẩn xác
            variant.IsDeleted = true;

            // Xóa Sku để giải phóng Sku cho các mặt hàng mới (giống logic giải phóng Email)
            if (!string.IsNullOrEmpty(variant.Sku))
            {
                variant.Sku = $"{variant.Sku}_deleted_{DateTime.UtcNow.Ticks}";
            }

            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();
        }
    }
}