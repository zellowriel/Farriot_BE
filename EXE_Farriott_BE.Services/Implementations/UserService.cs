using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo) => _userRepo = userRepo;

        private UserResponse MapToResponse(User u) => new UserResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        };

        public async Task<PagedResult<UserResponse>> GetUsersAsync(BaseQueryParameters query)
        {
            var (items, total) = await _userRepo.GetAllAsync(query);
            return new PagedResult<UserResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalItems = total,
                Page = query.Page,
                Size = query.Size ?? (total > 0 ? total : 1)
            };
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var u = await _userRepo.GetUserByIdAsync(id);
            return u == null ? null : MapToResponse(u);
        }

        public async Task<(bool IsSuccess, string ErrorMessage, UserResponse? Data)> CreateUserAsync(CreateUserRequest request)
        {
            // Kiểm tra trùng Email
            var existingUser = await _userRepo.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return (false, "Email này đã được sử dụng.", null);
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                // Dùng BCrypt để mã hóa (Giống hệt cách bạn làm ở Auth)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Role = request.Role,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
            };

            var createdUser = await _userRepo.CreateUserAsync(user);
            return (true, string.Empty, MapToResponse(createdUser));
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return (false, "Không tìm thấy người dùng.");

            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.Role = request.Role;

            await _userRepo.UpdateUserAsync(user);
            return (true, string.Empty);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return false;

            await _userRepo.DeleteAsync(user);
            return true;
        }
    }
}