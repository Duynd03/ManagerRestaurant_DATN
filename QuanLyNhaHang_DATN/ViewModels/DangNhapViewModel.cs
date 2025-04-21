using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        [Display(Name = "Tên đăng nhập")]
        public string TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }
    }
}
