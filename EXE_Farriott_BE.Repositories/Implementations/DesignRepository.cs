using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EXE_Farriott_BE.Repositories.Implementations
{
    public class DesignRepository : IDesignRepository
    {
        private readonly AppDbContext _context;

        public DesignRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Design> items, int total)> GetAllAsync(BaseQueryParameters query)
        {
            // Lọc bỏ các mẫu in đã bị tắt (Xóa mềm)
            var dbQuery = _context.Designs.Where(d => d.IsActive == true).AsQueryable();

            dbQuery = dbQuery.ApplyDynamicSearch(query.SearchBy, query.Keyword);

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                bool isDesc = query.SortBy.StartsWith("-");
                string sortCol = isDesc ? query.SortBy.Substring(1).ToLower() : query.SortBy.ToLower();

                dbQuery = sortCol switch
                {
                    "price" or "pricemodifier" => isDesc ? dbQuery.OrderByDescending(d => d.PriceModifier) : dbQuery.OrderBy(d => d.PriceModifier),
                    "name" => isDesc ? dbQuery.OrderByDescending(d => d.Name) : dbQuery.OrderBy(d => d.Name),
                    "createdat" => isDesc ? dbQuery.OrderByDescending(d => d.CreatedAt) : dbQuery.OrderBy(d => d.CreatedAt),
                    _ => isDesc ? dbQuery.OrderByDescending(d => d.Id) : dbQuery.OrderBy(d => d.Id)
                };
            }
            else
            {
                dbQuery = dbQuery.OrderByDescending(d => d.CreatedAt);
            }

            int total = await dbQuery.CountAsync();

            List<Design> items;
            if (query.Size.HasValue && query.Size.Value > 0)
            {
                items = await dbQuery.Skip((query.Page - 1) * query.Size.Value).Take(query.Size.Value).ToListAsync();
            }
            else
            {
                items = await dbQuery.ToListAsync();
            }

            return (items, total);
        }

        public async Task<Design?> GetByIdAsync(int id)
        {
            return await _context.Designs.FirstOrDefaultAsync(d => d.Id == id && d.IsActive == true);
        }

        public async Task<Design> CreateAsync(Design design)
        {
            _context.Designs.Add(design);
            await _context.SaveChangesAsync();
            return design;
        }

        public async Task UpdateAsync(Design design)
        {
            _context.Designs.Update(design);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Design design)
        {
            // Xóa mềm bằng cách tắt cờ IsActive
            design.IsActive = false;
            _context.Designs.Update(design);
            await _context.SaveChangesAsync();
        }
    }
}