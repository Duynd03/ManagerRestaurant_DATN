using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class DangKyViewModel
    {
        // Thông tin cho KhachHang
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [Display(Name = "Tên khách hàng")]
        public string TenKhachHang { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        // Thông tin cho TaiKhoan
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        [Display(Name = "Tên đăng nhập")]
        public string TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 8)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Nhập lại mật khẩu")]
        public string NhapLaiMatKhau { get; set; }

        [Required(ErrorMessage = "Bạn phải đồng ý với điều khoản dịch vụ")]
        [Display(Name = "Đồng ý điều khoản")]
        public bool AgreeTerms { get; set; }
    }
}
