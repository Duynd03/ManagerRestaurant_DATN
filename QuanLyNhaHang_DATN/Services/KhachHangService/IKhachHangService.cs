using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.KhachHangService
{
    public interface IKhachHangService : IBaseService<KhachHang>
    {
        Task<Result<KhachHang>> TaoKhachHangAsync(string tenKhachHang, string sdt, int? taiKhoanId, string diaChi, string email);
        Task<KhachHang> GetByTaiKhoanUsernameAsync(string username);
    }
}
