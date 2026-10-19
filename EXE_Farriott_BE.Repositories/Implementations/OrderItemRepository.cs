using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EXE_Farriott_BE.Repositories.Implementations
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;
        public OrderItemRepository(AppDbContext context) => _context = context;

        public async Task<OrderItem?> GetByIdAsync(int id) => await _context.OrderItems.FindAsync(id);
        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
        }
    }
}