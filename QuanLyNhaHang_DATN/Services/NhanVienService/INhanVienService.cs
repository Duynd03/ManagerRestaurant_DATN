using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.NhanVienService
{
    public interface INhanVienService : IBaseService<NhanVien>
    {
        Task<NhanVien> GetByTaiKhoanUsernameAsync(string username);
        Task<Result<NhanVien>> TaoNhanVienAsync(string tenNhanVien, string sdt, DateTime? ngaySinh, string diaChi, int taiKhoanId);
    }
}
