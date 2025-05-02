using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Areas.Admin.ViewModels
{
    public class DatBanFilterModel
    {
        public string? TenKhachHang { get; set; }
        public string? SDT { get; set; }
        public TrangThaiBanDat? TrangThai { get; set; }
    }
}
