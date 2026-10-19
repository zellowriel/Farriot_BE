using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool IsSuccess, string ErrorMessage, LoginResponse? Data)> LoginAsync(LoginRequest request);

        Task<(bool IsSuccess, string ErrorMessage)> RegisterAsync(RegisterRequest request);
        Task<(bool IsSuccess, string ErrorMessage)> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}