using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EXE_Farriott_BE.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Product> items, int total)> GetAllAsync(BaseQueryParameters query)
        {
            // 1. QUAN TRỌNG: Lọc bỏ ngay các sản phẩm đã bị xóa mềm từ đầu
            var dbQuery = _context.Products.Where(p => !p.IsDeleted).AsQueryable();

            // 2. Expand (Eager Loading)
            if (!string.IsNullOrEmpty(query.Expand) && query.Expand.ToLower().Contains("variants"))
            {
                dbQuery = dbQuery.Include(p => p.ProductVariants);
            }

            // 3. Search
            dbQuery = dbQuery.ApplyDynamicSearch(query.SearchBy, query.Keyword);

            // 4. Sort
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                bool isDesc = query.SortBy.StartsWith("-");
                string sortCol = isDesc ? query.SortBy.Substring(1).ToLower() : query.SortBy.ToLower();

                dbQuery = sortCol switch
                {
                    "price" => isDesc ? dbQuery.OrderByDescending(p => p.BasePrice) : dbQuery.OrderBy(p => p.BasePrice),
                    "createdat" => isDesc ? dbQuery.OrderByDescending(p => p.CreatedAt) : dbQuery.OrderBy(p => p.CreatedAt),
                    "name" => isDesc ? dbQuery.OrderByDescending(p => p.Name) : dbQuery.OrderBy(p => p.Name),
                    _ => isDesc ? dbQuery.OrderByDescending(p => p.Id) : dbQuery.OrderBy(p => p.Id)
                };
            }
            else
            {
                // Sort mặc định nếu không truyền
                dbQuery = dbQuery.OrderByDescending(p => p.CreatedAt);
            }

            // Đếm tổng số lượng record thỏa mãn điều kiện
            int total = await dbQuery.CountAsync();

            // Xử lý Paging (Phân trang)
            List<Product> items;

            if (query.Size.HasValue && query.Size.Value > 0)
            {
                // Nếu có nhập Size -> Thực hiện cắt trang (Skip và Take)
                items = await dbQuery
                    .Skip((query.Page - 1) * query.Size.Value)
                    .Take(query.Size.Value)
                    .ToListAsync();
            }
            else
            {
                // CỰC KỲ QUAN TRỌNG: Nếu Size = null -> LẤY TẤT CẢ (Không cắt)
                items = await dbQuery.ToListAsync();
            }

            return (items, total);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            // 5. CẬP NHẬT: Hàm Get này sẽ chặn luôn việc lấy sản phẩm đã bị xóa
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            // 6. CẬP NHẬT: Đây mới là logic Xóa mềm chuẩn!
            product.IsDeleted = true; // Đổi cờ
            _context.Products.Update(product); // Cập nhật lại trạng thái
            await _context.SaveChangesAsync(); // Lưu xuống Database
        }
    }
}