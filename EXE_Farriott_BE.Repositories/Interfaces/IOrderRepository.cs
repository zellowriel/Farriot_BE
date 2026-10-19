using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;

namespace EXE_Farriott_BE.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<(List<Order> items, int total)> GetAllAsync(BaseQueryParameters query, int? userId = null);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByCodeAsync(string orderCode); // Dùng cho tra cứu khách vãng lai
        Task<Order> CreateAsync(Order order);
        Task UpdateAsync(Order order);
    }
}