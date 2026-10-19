using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/custom-prints")]
    [ApiController]
    public class CustomPrintsController : ControllerBase
    {
        private readonly ICustomPrintService _printService;

        public CustomPrintsController(ICustomPrintService printService) => _printService = printService;

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomPrint(int id, [FromBody] UpdateCustomPrintRequest request)
        {
            var result = await _printService.UpdateCustomPrintAsync(id, request);
            if (!result.IsSuccess) return NotFound(new { message = result.ErrorMessage });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomPrint(int id)
        {
            var success = await _printService.DeleteCustomPrintAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy bản in thiết kế." });
            return NoContent();
        }
    }
}