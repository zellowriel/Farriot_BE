using System.ComponentModel.DataAnnotations;

namespace EXE_Farriott_BE.Services.DTOs
{
    public class ProductVariantResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string? Sku { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Tùy chọn: Trả về thêm tên sản phẩm cha để Frontend dễ hiển thị
        public string? ProductName { get; set; }
    }

    public class CreateProductVariantRequest
    {
        [Required(ErrorMessage = "Mã sản phẩm cha (ProductId) không được để trống.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Màu sắc không được để trống.")]
        [MaxLength(50, ErrorMessage = "Màu sắc không được vượt quá 50 ký tự.")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Kích thước (Size) không được để trống.")]
        [MaxLength(20, ErrorMessage = "Kích thước không được vượt quá 20 ký tự.")]
        public string Size { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm.")]
        public int StockQuantity { get; set; } = 0;

        [MaxLength(100, ErrorMessage = "Mã SKU không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9\-_]*$", ErrorMessage = "Mã SKU chỉ được chứa chữ cái không dấu, số, dấu gạch ngang (-) và gạch dưới (_).")]
        public string? Sku { get; set; }

        [Url(ErrorMessage = "Đường dẫn ảnh (ImageUrl) không hợp lệ.")]
        public string? ImageUrl { get; set; }
    }

    public class UpdateProductVariantRequest
    {
        [Required(ErrorMessage = "Màu sắc không được để trống.")]
        [MaxLength(50, ErrorMessage = "Màu sắc không được vượt quá 50 ký tự.")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Kích thước (Size) không được để trống.")]
        [MaxLength(20, ErrorMessage = "Kích thước không được vượt quá 20 ký tự.")]
        public string Size { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm.")]
        public int StockQuantity { get; set; }

        [MaxLength(100, ErrorMessage = "Mã SKU không được vượt quá 100 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9\-_]*$", ErrorMessage = "Mã SKU chỉ được chứa chữ cái không dấu, số, dấu gạch ngang (-) và gạch dưới (_).")]
        public string? Sku { get; set; }

        [Url(ErrorMessage = "Đường dẫn ảnh (ImageUrl) không hợp lệ.")]
        public string? ImageUrl { get; set; }
    }
}