using System.ComponentModel.DataAnnotations;

namespace EXE_Farriott_BE.Services.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải dài ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu bắt buộc phải có ít nhất 1 chữ cái và 1 chữ số.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp với mật khẩu đã nhập.")]
        public string ConfirmPassword { get; set; } = null!;

        public string? Phone { get; set; }
    }
}