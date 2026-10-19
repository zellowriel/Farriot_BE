using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/order-items")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _itemService;

        public OrderItemsController(IOrderItemService itemService) => _itemService = itemService;

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] UpdateOrderItemRequest request)
        {
            var result = await _itemService.UpdateOrderItemAsync(id, request);
            if (!result.IsSuccess) return NotFound(new { message = result.ErrorMessage });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var success = await _itemService.DeleteOrderItemAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng." });
            return NoContent();
        }
    }
}