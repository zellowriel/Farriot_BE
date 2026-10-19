using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserResponse>> GetUsersAsync(BaseQueryParameters query);
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<(bool IsSuccess, string ErrorMessage, UserResponse? Data)> CreateUserAsync(CreateUserRequest request);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
    }
}