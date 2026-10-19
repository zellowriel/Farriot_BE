using EXE_Farriott_BE.Repositories.Commons;
using EXE_Farriott_BE.Repositories.Entities;
using EXE_Farriott_BE.Repositories.Interfaces;
using EXE_Farriott_BE.Services.DTOs;
using EXE_Farriott_BE.Services.Interfaces;

namespace EXE_Farriott_BE.Services.Implementations
{
    public class DesignService : IDesignService
    {
        private readonly IDesignRepository _designRepo;

        public DesignService(IDesignRepository designRepo)
        {
            _designRepo = designRepo;
        }

        public async Task<PagedResult<DesignResponse>> GetDesignsAsync(BaseQueryParameters query)
        {
            var (items, total) = await _designRepo.GetAllAsync(query);

            var responseItems = items.Select(d => new DesignResponse
            {
                Id = d.Id,
                Name = d.Name,
                ImageUrl = d.ImageUrl,
                PriceModifier = d.PriceModifier ?? 0,
                IsActive = d.IsActive ?? false,
                CreatedAt = d.CreatedAt
            }).ToList();

            return new PagedResult<DesignResponse>
            {
                Items = responseItems,
                TotalItems = total,
                Page = query.Page,
                Size = query.Size ?? (total > 0 ? total : 1)
            };
        }

        public async Task<DesignResponse?> GetDesignByIdAsync(int id)
        {
            var d = await _designRepo.GetByIdAsync(id);
            if (d == null) return null;

            return new DesignResponse
            {
                Id = d.Id,
                Name = d.Name,
                ImageUrl = d.ImageUrl,
                PriceModifier = d.PriceModifier ?? 0,
                IsActive = d.IsActive ?? false,
                CreatedAt = d.CreatedAt
            };
        }

        public async Task<(bool IsSuccess, string ErrorMessage, DesignResponse? Data)> CreateDesignAsync(CreateDesignRequest request)
        {
            var design = new Design
            {
                Name = request.Name,
                ImageUrl = request.ImageUrl,
                PriceModifier = request.PriceModifier,
                IsActive = request.IsActive,
                // Lưu chuẩn giờ VN
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
            };

            var created = await _designRepo.CreateAsync(design);
            return (true, string.Empty, await GetDesignByIdAsync(created.Id));
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateDesignAsync(int id, UpdateDesignRequest request)
        {
            var design = await _designRepo.GetByIdAsync(id);
            if (design == null) return (false, "Không tìm thấy mẫu in.");

            design.Name = request.Name;
            design.ImageUrl = request.ImageUrl;
            design.PriceModifier = request.PriceModifier;
            design.IsActive = request.IsActive;

            await _designRepo.UpdateAsync(design);
            return (true, string.Empty);
        }

        public async Task<bool> DeleteDesignAsync(int id)
        {
            var design = await _designRepo.GetByIdAsync(id);
            if (design == null) return false;

            await _designRepo.DeleteAsync(design);
            return true;
        }
    }
}