using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int TaiKhoanId { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
