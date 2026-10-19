using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EXE_Farriott_BE.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Order> items, int total)> GetAllAsync(BaseQueryParameters query, int? userId = null)
        {
            var dbQuery = _context.Orders
                .Include(o => o.OrderItems) // Load kèm Item để đếm hoặc tính toán sơ bộ nếu cần
                .AsQueryable();

            if (userId.HasValue)
            {
                dbQuery = dbQuery.Where(o => o.UserId == userId.Value);
            }

            dbQuery = dbQuery.ApplyDynamicSearch(query.SearchBy, query.Keyword);

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                bool isDesc = query.SortBy.StartsWith("-");
                string sortCol = isDesc ? query.SortBy.Substring(1).ToLower() : query.SortBy.ToLower();

                dbQuery = sortCol switch
                {
                    "amount" or "totalamount" => isDesc ? dbQuery.OrderByDescending(o => o.TotalAmount) : dbQuery.OrderBy(o => o.TotalAmount),
                    "status" => isDesc ? dbQuery.OrderByDescending(o => o.Status) : dbQuery.OrderBy(o => o.Status),
                    "createdat" => isDesc ? dbQuery.OrderByDescending(o => o.CreatedAt) : dbQuery.OrderBy(o => o.CreatedAt),
                    _ => isDesc ? dbQuery.OrderByDescending(o => o.Id) : dbQuery.OrderBy(o => o.Id)
                };
            }
            else
            {
                dbQuery = dbQuery.OrderByDescending(o => o.CreatedAt);
            }

            int total = await dbQuery.CountAsync();

            List<Order> items;
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

        public async Task<Order?> GetByIdAsync(int id)
        {
            // Lấy Full cục dữ liệu JSON lồng nhau
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.CustomPrints)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByCodeAsync(string orderCode)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.CustomPrints)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            // Điểm ăn tiền của EF Core: Truyền Order có chứa List OrderItems, 
            // nó sẽ tự động sinh Transaction, Insert bảng Cha rồi Insert bảng Con
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}