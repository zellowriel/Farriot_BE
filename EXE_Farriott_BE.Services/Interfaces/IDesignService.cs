using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Services.DTOs;

namespace EXE_Farriott_BE.Services.Interfaces
{
    public interface IDesignService
    {
        Task<PagedResult<DesignResponse>> GetDesignsAsync(BaseQueryParameters query);
        Task<DesignResponse?> GetDesignByIdAsync(int id);
        Task<(bool IsSuccess, string ErrorMessage, DesignResponse? Data)> CreateDesignAsync(CreateDesignRequest request);
        Task<(bool IsSuccess, string ErrorMessage)> UpdateDesignAsync(int id, UpdateDesignRequest request);
        Task<bool> DeleteDesignAsync(int id);
    }
}