using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string ErrorMessage, LoginResponse? Data)> LoginAsync(LoginRequest request)
        {
            // 1. Tìm User qua Email
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                // Bảo mật: Không nên báo "Không tìm thấy Email", mà gộp chung với sai mật khẩu
                return (false, "Email hoặc mật khẩu không chính xác.", null);
            }

            // 2. KIỂM TRA KHÓA/XÓA MỀM TẠI ĐÂY
            if (user.IsDeleted)
            {
                return (false, "Tài khoản của bạn đã bị khóa hoặc vô hiệu hóa.", null);
            }

            // 3. Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return (false, "Email hoặc mật khẩu không chính xác.", null);
            }

            // 4. Tạo Token (Giữ nguyên code cũ của bạn)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!)),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var responseData = new LoginResponse
            {
                Token = tokenHandler.WriteToken(token),
                FullName = user.FullName,
                Role = user.Role
            };

            // 5. Trả về thành công
            return (true, string.Empty, responseData);
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(RegisterRequest request)
        {
            // 1. Kiểm tra Email đã tồn tại chưa
            if (await _userRepository.IsEmailExistsAsync(request.Email))
            {
                return (false, "Email này đã được sử dụng.");
            }

            // 2. Chuyển đổi từ DTO sang Entity, Gán mặc định Role và Mã hóa mật khẩu
            var newUser = new EXE_Farriott_BE.Repositories.Entities.User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // Băm mật khẩu!
                Phone = request.Phone,
                Role = "customer" // Ép buộc mặc định luôn là customer
            };

            // 3. Lưu xuống Database
            await _userRepository.CreateUserAsync(newUser);

            return (true, string.Empty);
        }
        public async Task<(bool IsSuccess, string ErrorMessage)> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            // 1. Tìm User theo ID
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return (false, "Người dùng không tồn tại.");
            }

            // 2. Kiểm tra mật khẩu hiện tại (CurrentPassword)
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return (false, "Mật khẩu hiện tại không chính xác.");
            }

            // 3. Kiểm tra mật khẩu mới không được trùng với mật khẩu cũ
            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
            {
                return (false, "Mật khẩu mới không được trùng với mật khẩu hiện tại.");
            }

            // 4. Băm mật khẩu mới và lưu xuống DB
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            return (true, string.Empty);
        }
    }
}