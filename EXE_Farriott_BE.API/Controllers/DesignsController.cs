using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/designs")]
    [ApiController]
    public class DesignsController : ControllerBase
    {
        private readonly IDesignService _designService;

        public DesignsController(IDesignService designService)
        {
            _designService = designService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDesigns([FromQuery] BaseQueryParameters query)
        {
            var result = await _designService.GetDesignsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDesign(int id)
        {
            var design = await _designService.GetDesignByIdAsync(id);
            if (design == null) return NotFound(new { message = "Không tìm thấy mẫu in." });
            return Ok(design);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDesign([FromBody] CreateDesignRequest request)
        {
            var result = await _designService.CreateDesignAsync(request);
            if (!result.IsSuccess) return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetDesign), new { id = result.Data?.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDesign(int id, [FromBody] UpdateDesignRequest request)
        {
            var result = await _designService.UpdateDesignAsync(id, request);
            if (!result.IsSuccess) return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesign(int id)
        {
            var success = await _designService.DeleteDesignAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy mẫu in." });

            return NoContent();
        }
    }
}