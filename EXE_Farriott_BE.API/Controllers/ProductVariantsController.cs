using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/product-variants")]
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly IProductVariantService _variantService;

        public ProductVariantsController(IProductVariantService variantService)
        {
            _variantService = variantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVariants([FromQuery] BaseQueryParameters query, [FromQuery] int? productId = null)
        {
            var result = await _variantService.GetVariantsAsync(query, productId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVariant(int id)
        {
            var variant = await _variantService.GetVariantByIdAsync(id);
            if (variant == null)
            {
                return NotFound(new { message = "Không tìm thấy biến thể sản phẩm." });
            }
            return Ok(variant);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVariant([FromBody] CreateProductVariantRequest request)
        {
            var result = await _variantService.CreateVariantAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return CreatedAtAction(nameof(GetVariant), new { id = result.Data?.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVariant(int id, [FromBody] UpdateProductVariantRequest request)
        {
            var result = await _variantService.UpdateVariantAsync(id, request);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage.Contains("Không tìm thấy")) return NotFound(new { message = result.ErrorMessage });
                return BadRequest(new { message = result.ErrorMessage });
            }

            return NoContent(); // Trả về 204 No Content báo hiệu update thành công
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var success = await _variantService.DeleteVariantAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy biến thể sản phẩm." });
            }

            return NoContent();
        }
    }
}