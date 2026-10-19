using System.ComponentModel.DataAnnotations;

namespace EXE_Farriott_BE.Services.DTOs
{
    // --- RESPONSE DTOs ---
    public class OrderResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string OrderCode { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }

    public class OrderItemResponse
    {
        public int Id { get; set; }
        public int VariantId { get; set; }
        public int? DesignId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public List<CustomPrintResponse> CustomPrints { get; set; } = new();
    }

    public class CustomPrintResponse
    {
        public int Id { get; set; }
        public string UploadedImageUrl { get; set; } = null!;
        public string? PrintMetadata { get; set; }
        public string? CustomerNotes { get; set; }
    }

    // --- REQUEST DTOs ---
    public class CreateOrderRequest
    {
        public int? UserId { get; set; } // Khách vãng lai truyền null

        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống.")]
        public string ShippingAddress { get; set; } = null!;

        [Required(ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm.")]
        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm.")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        [Required]
        public int VariantId { get; set; }

        public int? DesignId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ.")]
        public decimal UnitPrice { get; set; }

        // Có thể rỗng nếu khách chọn mẫu in sẵn (DesignId)
        public List<CreateCustomPrintRequest> CustomPrints { get; set; } = new();
    }

    public class CreateCustomPrintRequest
    {
        [Required(ErrorMessage = "Ảnh tải lên không được để trống.")]
        [Url(ErrorMessage = "Đường dẫn ảnh không hợp lệ.")]
        public string UploadedImageUrl { get; set; } = null!;

        public string? PrintMetadata { get; set; } // Chuỗi JSON tọa độ
        public string? CustomerNotes { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        [RegularExpression("^(pending|processing|shipped|delivered|cancelled|refunded)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; } = null!;
    }

    public class UpdateOrderItemRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm không hợp lệ.")]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateCustomPrintRequest
    {
        [Required(ErrorMessage = "Ảnh tải lên không được để trống.")]
        [Url(ErrorMessage = "Đường dẫn ảnh không hợp lệ.")]
        public string UploadedImageUrl { get; set; } = null!;

        public string? PrintMetadata { get; set; }
        public string? CustomerNotes { get; set; }
    }
}