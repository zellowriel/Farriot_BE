using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EXE_Farriott_BE.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            // Nếu thất bại (Bị khóa hoặc Sai pass)
            if (!result.IsSuccess)
            {
                // Trả về JSON: { "message": "Tài khoản của bạn đã bị khóa hoặc vô hiệu hóa." }
                return BadRequest(new { message = result.ErrorMessage });
            }

            // Nếu thành công
            return Ok(result.Data);
        }

        // HÀM ĐĂNG KÝ
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (isSuccess, errorMessage) = await _authService.RegisterAsync(request);

            if (!isSuccess)
                return BadRequest(new { message = errorMessage }); // Lỗi 400

            // REST API chuẩn: Báo thành công 201 (Created) hoặc 200 (Ok)
            return Ok(new { message = "Đăng ký tài khoản thành công!" });
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Lấy UserId từ Claims trong JWT Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Xác thực không hợp lệ." });
            }

            var (isSuccess, errorMessage) = await _authService.ChangePasswordAsync(userId, request);

            if (!isSuccess)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Đổi mật khẩu thành công!" });
        }
    }
}