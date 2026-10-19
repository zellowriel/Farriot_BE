using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EXE_Farriott_BE.Repositories.Implementations
{
    public class CustomPrintRepository : ICustomPrintRepository
    {
        private readonly AppDbContext _context;
        public CustomPrintRepository(AppDbContext context) => _context = context;

        public async Task<CustomPrint?> GetByIdAsync(int id) => await _context.CustomPrints.FindAsync(id);
        public async Task UpdateAsync(CustomPrint customPrint)
        {
            _context.CustomPrints.Update(customPrint);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(CustomPrint customPrint)
        {
            _context.CustomPrints.Remove(customPrint);
            await _context.SaveChangesAsync();
        }
    }
}