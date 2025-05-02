using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên tài khoản không được vượt quá 50 ký tự")]
        [Display(Name = "Tên tài khoản")]
        public string TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }
    }
}