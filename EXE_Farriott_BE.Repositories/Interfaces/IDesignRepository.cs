using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;

namespace EXE_Farriott_BE.Repositories.Interfaces
{
    public interface IDesignRepository
    {
        Task<(List<Design> items, int total)> GetAllAsync(BaseQueryParameters query);
        Task<Design?> GetByIdAsync(int id);
        Task<Design> CreateAsync(Design design);
        Task UpdateAsync(Design design);
        Task DeleteAsync(Design design);
    }
}