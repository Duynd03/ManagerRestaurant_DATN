using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class CreateNhanVienViewModel
    {
        [Required(ErrorMessage = "Tên nhân viên là bắt buộc")]
        public string TenNhanVien { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        public string Sdt { get; set; }

        public DateTime? NgaySinh { get; set; }
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Quyền là bắt buộc")]
        public int QuyenId { get; set; }
    }
}