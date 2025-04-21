using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.TaiKhoanRepository
{
    public class TaiKhoanRepository : BaseRepository<TaiKhoan>, ITaiKhoanRepository
    {
        public TaiKhoanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<TaiKhoan> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.TenTaiKhoan == username);
        }

        public async Task<bool> CheckExistUsernameAsync(string username)
        {
            return await _dbSet.AnyAsync(t => t.TenTaiKhoan == username);
        }
    }
}
