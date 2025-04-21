using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.KhachHangRepository
{
    public interface IKhachHangRepository : IBaseRepository<KhachHang>
    {
        Task<KhachHang> GetByTaiKhoanIdAsync(int taiKhoanId);
        Task<bool> CheckExistEmailAsync(string email);
    }
}
