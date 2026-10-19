using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization; // BƯỚC 1: Thêm namespace này
using Microsoft.AspNetCore.Mvc;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "admin")] // BƯỚC 2: Thêm chốt chặn quyền Admin tại đây
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) => _userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] BaseQueryParameters query)
        {
            var result = await _userService.GetUsersAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng." });
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            if (!result.IsSuccess) return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            if (!result.IsSuccess) return NotFound(new { message = result.ErrorMessage });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy người dùng." });

            return NoContent();
        }
    }
}