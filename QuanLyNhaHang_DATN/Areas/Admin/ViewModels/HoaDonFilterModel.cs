using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Areas.Admin.ViewModels
{
    public class HoaDonFilterModel
    {
        public string? TenKhachHang { get; set; }
        public string? SDT { get; set; }
        public string MaHoaDon { get; set; }
        public TrangThaiHoaDon? TrangThai { get; set; }
    }
}
