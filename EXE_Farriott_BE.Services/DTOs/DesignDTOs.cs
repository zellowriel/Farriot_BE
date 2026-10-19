using System.ComponentModel.DataAnnotations;

namespace EXE_Farriott_BE.Services.DTOs
{
    public class DesignResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal PriceModifier { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateDesignRequest
    {
        [Required(ErrorMessage = "Tên mẫu in không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên mẫu in không được vượt quá 255 ký tự.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Đường dẫn ảnh mẫu in không được để trống.")]
        [Url(ErrorMessage = "Đường dẫn ảnh (ImageUrl) không hợp lệ.")]
        public string ImageUrl { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Phụ phí (Price Modifier) không được âm.")]
        public decimal PriceModifier { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateDesignRequest
    {
        [Required(ErrorMessage = "Tên mẫu in không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên mẫu in không được vượt quá 255 ký tự.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Đường dẫn ảnh mẫu in không được để trống.")]
        [Url(ErrorMessage = "Đường dẫn ảnh (ImageUrl) không hợp lệ.")]
        public string ImageUrl { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "Phụ phí (Price Modifier) không được âm.")]
        public decimal PriceModifier { get; set; }

        public bool IsActive { get; set; }
    }
}