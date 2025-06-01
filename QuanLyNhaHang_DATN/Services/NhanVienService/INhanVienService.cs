using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Controllers;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.NhanVienService
{
    public interface INhanVienService : IBaseService<NhanVien>
    {
        Task<NhanVien> GetByTaiKhoanUsernameAsync(string username);
        Task<Result<NhanVien>> TaoNhanVienAsync(string tenNhanVien, string sdt, DateTime? ngaySinh, string diaChi, int taiKhoanId);
        Task<(IEnumerable<NhanVien> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, NhanVienFilterModel filter);
        Task<NhanVien> GetByIdAsync(int id);
    }
}
