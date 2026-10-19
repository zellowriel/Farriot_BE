using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] BaseQueryParameters query, [FromQuery] int? userId = null)
        {
            var result = await _orderService.GetOrdersAsync(query, userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng." });
            return Ok(order);
        }

        [HttpGet("track/{orderCode}")]
        public async Task<IActionResult> TrackOrder(string orderCode)
        {
            var order = await _orderService.TrackOrderAsync(orderCode);
            if (order == null) return NotFound(new { message = "Không tìm thấy mã đơn hàng này." });
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);
            if (!result.IsSuccess) return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetOrder), new { id = result.Data?.Id }, result.Data);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, request.Status);
            if (!result.IsSuccess) return BadRequest(new { message = result.ErrorMessage });

            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            // Dùng PUT thay vì DELETE vì đây là hành động cập nhật trạng thái nghiệp vụ
            var result = await _orderService.CancelOrderAsync(id);
            if (!result.IsSuccess) return BadRequest(new { message = result.ErrorMessage });

            return NoContent();
        }
    }
}