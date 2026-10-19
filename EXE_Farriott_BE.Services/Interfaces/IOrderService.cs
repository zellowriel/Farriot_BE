using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResult<OrderResponse>> GetOrdersAsync(BaseQueryParameters query, int? userId = null);
        Task<OrderResponse?> GetOrderByIdAsync(int id);
        Task<OrderResponse?> TrackOrderAsync(string orderCode);
        Task<(bool IsSuccess, string ErrorMessage, OrderResponse? Data)> CreateOrderAsync(CreateOrderRequest request);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateOrderStatusAsync(int id, string newStatus);
        Task<(bool IsSuccess, string ErrorMessage)> CancelOrderAsync(int id);
    }
}