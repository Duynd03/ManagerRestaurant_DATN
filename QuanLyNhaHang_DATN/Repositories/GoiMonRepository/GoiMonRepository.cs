using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.GoiMonRepository
{
    public class GoiMonRepository : BaseRepository<GoiMon>, IGoiMonRepository
    {
        public GoiMonRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<GoiMon>> GetByDatBanIdAsync(int datBanId)
        {
            return await _dbSet
                .Where(gm => gm.DatBanId == datBanId)
                .Include(gm => gm.MonAn)
                 .Include(gm => gm.DatBan)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoiMon>> GetPagedByDatBanIdAsync(int datBanId, int pageIndex, int pageSize)
        {
            return await _dbSet
                .Where(gm => gm.DatBanId == datBanId)
                .Include(gm => gm.MonAn)
                .Include(gm => gm.DatBan)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<decimal> CalculateTongTienByDatBanIdAsync(int datBanId)
        {
            var goiMons = await _dbSet
                .Where(gm => gm.DatBanId == datBanId)
                .ToListAsync();
            return goiMons.Sum(gm => gm.Gia * gm.SoLuong);
        }
    }
}