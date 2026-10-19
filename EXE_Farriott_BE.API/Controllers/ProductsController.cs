using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // Bất kỳ ai cũng xem được sản phẩm (kể cả khách vãng lai chưa login)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] BaseQueryParameters query)
        {
            var result = await _productService.GetProductsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });
            return Ok(product);
        }

        // Chỉ Admin mới được quyền tạo, sửa, xóa
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
        {
            var isSuccess = await _productService.UpdateProductAsync(id, request);
            if (!isSuccess) return NotFound(new { message = "Không tìm thấy sản phẩm để cập nhật." });

            return Ok(new { message = "Cập nhật sản phẩm thành công." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var isSuccess = await _productService.DeleteProductAsync(id);
            if (!isSuccess) return NotFound(new { message = "Không tìm thấy sản phẩm để xóa." });

            return Ok(new { message = "Xóa sản phẩm thành công." });
        }
    }
}