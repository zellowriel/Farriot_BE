using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<(bool IsSuccess, string ErrorMessage)> UpdateOrderItemAsync(int id, UpdateOrderItemRequest request);
        Task<bool> DeleteOrderItemAsync(int id);
    }
}