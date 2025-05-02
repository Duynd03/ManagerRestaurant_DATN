using Microsoft.AspNetCore.Identity;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.TaiKhoanRepository
{
    public interface ITaiKhoanRepository : IBaseRepository<TaiKhoan>
    {
        Task<TaiKhoan> GetByUsernameAsync(string username);
        Task<bool> CheckExistUsernameAsync(string username);
        Task<IdentityResult> CreateAsync(TaiKhoan taiKhoan, string password);
    }
}
