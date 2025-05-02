using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
namespace QuanLyNhaHang_DATN.Repositories.BanRepository
{
    public class BanRepository : BaseRepository<Ban>, IBanRepository
    {
        public BanRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Ban>> GetByKhuVucBanAsync(int khuVucBanId)
        {
            return await _dbSet.Where(m => m.KhuVucBanId == khuVucBanId).ToListAsync();
        }

        public async Task<IEnumerable<Ban>> GetAvailableAsync()
        {
            return await _dbSet
                .Where(m => m.TrangThai == TrangThaiBan.Trong)
                .Include(m => m.KhuVucBan)
                .ToListAsync();
        }

    }
}
