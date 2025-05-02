using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class DangKyViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên tài khoản không được vượt quá 50 ký tự")]
        [Display(Name = "Tên tài khoản")]
        public string TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
        [RegularExpression(@"^(?=.*\d).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất một số")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string NhapLaiMatKhau { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        [Display(Name = "Tên khách hàng")]
        public string TenKhachHang { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
        [Required(ErrorMessage = "Bạn phải đồng ý với điều khoản dịch vụ")]
        public bool AgreeTerms { get; set; } // Thêm thuộc tính này
    }
}