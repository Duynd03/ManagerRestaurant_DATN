using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class UpdateNhanVienViewModel
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Tên nhân viên là bắt buộc")]
        public string TenNhanVien { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        public string Sdt { get; set; }

        public DateTime? NgaySinh { get; set; }
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "Quyền là bắt buộc")]
        public int QuyenId { get; set; }
    }
}