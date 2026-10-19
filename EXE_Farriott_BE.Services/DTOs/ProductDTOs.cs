using System.ComponentModel.DataAnnotations;

namespace EXE_Farriott_BE.Services.DTOs
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsCustomizable { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateProductRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsCustomizable { get; set; } = true;
    }

    public class UpdateProductRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }

        public bool IsCustomizable { get; set; }
    }
}