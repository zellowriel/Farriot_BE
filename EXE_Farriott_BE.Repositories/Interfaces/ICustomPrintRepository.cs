using EXE_Farriott_BE.Repositories.Entities;

namespace EXE_Farriott_BE.Repositories.Interfaces
{
    public interface ICustomPrintRepository
    {
        Task<CustomPrint?> GetByIdAsync(int id);
        Task UpdateAsync(CustomPrint customPrint);
        Task DeleteAsync(CustomPrint customPrint);
    }
}
