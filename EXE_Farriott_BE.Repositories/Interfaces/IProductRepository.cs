using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Commons;

namespace EXE_Farriott_BE.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<Product> items, int total)> GetAllAsync(BaseQueryParameters query);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}