using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface ICustomPrintService
    {
        Task<(bool IsSuccess, string ErrorMessage)> UpdateCustomPrintAsync(int id, UpdateCustomPrintRequest request);
        Task<bool> DeleteCustomPrintAsync(int id);
    }
}