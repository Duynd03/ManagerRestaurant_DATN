using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.NhanVienRepository
{
    public interface INhanVienRepository : IBaseRepository<NhanVien>
    {
        // Lấy thông tin nhân viên dựa trên username của tài khoản
        Task<NhanVien> GetByTaiKhoanUsernameAsync(string username);

        // Lấy thông tin nhân viên dựa trên TaiKhoanId
        Task<NhanVien> GetByTaiKhoanIdAsync(int taiKhoanId);

        // Kiểm tra số điện thoại đã tồn tại chưa
        Task<bool> CheckExistSdtAsync(string sdt);
    }
}
