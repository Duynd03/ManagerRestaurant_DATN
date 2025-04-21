using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.KhachHangRepository
{
    public class KhachHangRepository : BaseRepository<KhachHang>, IKhachHangRepository
    {
        public KhachHangRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<KhachHang> GetByTaiKhoanIdAsync(int taiKhoanId)
        {
            return await _dbSet.FirstOrDefaultAsync(kh => kh.TaiKhoanId == taiKhoanId);
        }

        public async Task<bool> CheckExistEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(kh => kh.Email == email);
        }
    }
}
