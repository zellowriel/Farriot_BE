using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        private OrderResponse MapToResponse(Order o)
        {
            return new OrderResponse
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderCode = o.OrderCode,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    VariantId = oi.VariantId,
                    DesignId = oi.DesignId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    CustomPrints = oi.CustomPrints.Select(cp => new CustomPrintResponse
                    {
                        Id = cp.Id,
                        UploadedImageUrl = cp.UploadedImageUrl,
                        PrintMetadata = cp.PrintMetadata,
                        CustomerNotes = cp.CustomerNotes
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<PagedResult<OrderResponse>> GetOrdersAsync(BaseQueryParameters query, int? userId = null)
        {
            var (items, total) = await _orderRepo.GetAllAsync(query, userId);
            return new PagedResult<OrderResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalItems = total,
                Page = query.Page,
                Size = query.Size ?? (total > 0 ? total : 1)
            };
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id)
        {
            var o = await _orderRepo.GetByIdAsync(id);
            return o == null ? null : MapToResponse(o);
        }

        public async Task<OrderResponse?> TrackOrderAsync(string orderCode)
        {
            var o = await _orderRepo.GetByCodeAsync(orderCode);
            return o == null ? null : MapToResponse(o);
        }

        public async Task<(bool IsSuccess, string ErrorMessage, OrderResponse? Data)> CreateOrderAsync(CreateOrderRequest request)
        {
            // Tự động sinh mã đơn hàng (Ví dụ: ORD-20261019-XXXX)
            string randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper();
            string generatedOrderCode = $"ORD-{DateTime.Now:yyyyMMdd}-{randomSuffix}";

            // Tự tính tổng tiền từ Backend cho an toàn
            decimal totalAmount = request.Items.Sum(i => i.UnitPrice * i.Quantity);

            var newOrder = new Order
            {
                UserId = request.UserId,
                OrderCode = generatedOrderCode,
                TotalAmount = totalAmount,
                Status = "pending",
                ShippingAddress = request.ShippingAddress,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                OrderItems = request.Items.Select(i => new OrderItem
                {
                    VariantId = i.VariantId,
                    DesignId = i.DesignId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    CustomPrints = i.CustomPrints.Select(cp => new CustomPrint
                    {
                        UploadedImageUrl = cp.UploadedImageUrl,
                        PrintMetadata = cp.PrintMetadata,
                        CustomerNotes = cp.CustomerNotes,
                        CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                    }).ToList()
                }).ToList()
            };

            // DB tự lưu tất cả trong 1 Transaction ngầm
            var createdOrder = await _orderRepo.CreateAsync(newOrder);

            return (true, string.Empty, MapToResponse(createdOrder));
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateOrderStatusAsync(int id, string newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return (false, "Không tìm thấy đơn hàng.");

            order.Status = newStatus;
            await _orderRepo.UpdateAsync(order);

            return (true, string.Empty);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> CancelOrderAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return (false, "Không tìm thấy đơn hàng.");

            // Chặn hủy nếu đơn đã đi quá xa
            var notAllowedStatuses = new[] { "shipped", "delivered", "refunded", "cancelled" };
            if (notAllowedStatuses.Contains(order.Status.ToLower()))
            {
                return (false, $"Không thể hủy đơn hàng đang ở trạng thái: {order.Status}");
            }

            order.Status = "cancelled";
            await _orderRepo.UpdateAsync(order);

            return (true, string.Empty);
        }
    }
}