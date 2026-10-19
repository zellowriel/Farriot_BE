using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EXE_Farriott_BE.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // PHẦN 1: CÁC HÀM TỪ AUTH (ĐÃ NÂNG CẤP XÓA MỀM)
        // ==========================================

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // Chặn Login nếu tài khoản đã bị vô hiệu hóa
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            // Nếu email đã bị xóa mềm (đã đổi đuôi), nó sẽ trả về false, cho phép khách đăng ký lại
            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            // Đổi FindAsync thành FirstOrDefaultAsync để có thể kẹp thêm điều kiện IsDeleted
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // ==========================================
        // PHẦN 2: CÁC HÀM MỚI CHO TÍNH NĂNG CRUD ADMIN
        // ==========================================

        public async Task<(List<User> items, int total)> GetAllAsync(BaseQueryParameters query)
        {
            var dbQuery = _context.Users.Where(u => !u.IsDeleted).AsQueryable();

            dbQuery = dbQuery.ApplyDynamicSearch(query.SearchBy, query.Keyword);

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                bool isDesc = query.SortBy.StartsWith("-");
                string sortCol = isDesc ? query.SortBy.Substring(1).ToLower() : query.SortBy.ToLower();

                dbQuery = sortCol switch
                {
                    "name" or "fullname" => isDesc ? dbQuery.OrderByDescending(u => u.FullName) : dbQuery.OrderBy(u => u.FullName),
                    "email" => isDesc ? dbQuery.OrderByDescending(u => u.Email) : dbQuery.OrderBy(u => u.Email),
                    "createdat" => isDesc ? dbQuery.OrderByDescending(u => u.CreatedAt) : dbQuery.OrderBy(u => u.CreatedAt),
                    _ => isDesc ? dbQuery.OrderByDescending(u => u.Id) : dbQuery.OrderBy(u => u.Id)
                };
            }
            else
            {
                dbQuery = dbQuery.OrderByDescending(u => u.CreatedAt);
            }

            int total = await dbQuery.CountAsync();

            List<User> items;
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

        public async Task DeleteAsync(User user)
        {
            // Xóa mềm: Bật cờ và đổi hậu tố email để giải phóng email gốc
            user.IsDeleted = true;
            user.Email = $"{user.Email}_deleted_{DateTime.UtcNow.Ticks}";

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}