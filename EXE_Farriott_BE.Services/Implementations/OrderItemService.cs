using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _itemRepo;
        // private readonly IOrderRepository _orderRepo; // Tương lai bạn có thể dùng để tính toán lại TotalAmount của Order

        public OrderItemService(IOrderItemRepository itemRepo) => _itemRepo = itemRepo;

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateOrderItemAsync(int id, UpdateOrderItemRequest request)
        {
            var item = await _itemRepo.GetByIdAsync(id);
            if (item == null) return (false, "Không tìm thấy chi tiết đơn hàng.");

            item.Quantity = request.Quantity;
            item.UnitPrice = request.UnitPrice;

            await _itemRepo.UpdateAsync(item);
            return (true, string.Empty);
        }

        public async Task<bool> DeleteOrderItemAsync(int id)
        {
            var item = await _itemRepo.GetByIdAsync(id);
            if (item == null) return false;

            // Xóa cứng khỏi DB vì Item này bị bỏ khỏi đơn hàng
            await _itemRepo.DeleteAsync(item);
            return true;
        }
    }
}