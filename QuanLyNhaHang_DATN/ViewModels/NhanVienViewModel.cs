using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class NhanVienViewModel
    {
        public int Id { get; set; }
        public string TenNhanVien { get; set; }
        public string Sdt { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? DiaChi { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int QuyenId { get; set; }
        public string Quyen { get; set; }
        public TrangThaiTaiKhoan TrangThai { get; set; }
        public string TrangThaiDisplay { get; set; }
        public int? TaiKhoanId { get; set; }
    }
}
