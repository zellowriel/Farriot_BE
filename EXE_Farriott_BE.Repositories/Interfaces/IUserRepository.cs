using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;

namespace EXE_Farriott_BE.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // --- Các hàm của tính năng Auth (Giữ nguyên tên cũ) ---
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> IsEmailExistsAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task UpdateUserAsync(User user);

        // --- Các hàm bổ sung thêm cho tính năng Quản lý (Admin) ---
        Task<(List<User> items, int total)> GetAllAsync(BaseQueryParameters query);
        Task DeleteAsync(User user);
    }
}