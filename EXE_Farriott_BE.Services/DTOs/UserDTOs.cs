using System.ComponentModel.DataAnnotations;

namespace EXE_Farriott_BE.Services.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string Role { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        // TUYỆT ĐỐI KHÔNG trả về PasswordHash ra ngoài API
    }

    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(255, ErrorMessage = "Họ và tên không được vượt quá 255 ký tự.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải dài ít nhất 8 ký tự.")]
        public string Password { get; set; } = null!;

        [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string? Phone { get; set; }

        [RegularExpression("^(admin|customer)$", ErrorMessage = "Quyền (Role) chỉ được phép là 'admin' hoặc 'customer'.")]
        public string Role { get; set; } = "customer";
    }

    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(255, ErrorMessage = "Họ và tên không được vượt quá 255 ký tự.")]
        public string FullName { get; set; } = null!;

        [MaxLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        public string? Phone { get; set; }

        [RegularExpression("^(admin|customer)$", ErrorMessage = "Quyền (Role) chỉ được phép là 'admin' hoặc 'customer'.")]
        public string Role { get; set; } = "customer";

        // Cố tình không cho đổi Email và Password ở đây. Password đã có luồng ChangePassword riêng.
    }
}