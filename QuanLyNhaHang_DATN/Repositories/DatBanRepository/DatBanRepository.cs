using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.DatBanRepository
{
    public class DatBanRepository : BaseRepository<DatBan>, IDatBanRepository
    {
        public DatBanRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<DatBan> CreateAsync(DatBan datBan)
        {
            await _dbSet.AddAsync(datBan);
            await _context.SaveChangesAsync();
            return datBan;
        }
        public async Task<List<DatBan>> GetByTrangThaiAsync(TrangThaiBanDat trangThai)
        {
            return await _dbSet
                .Where(d => d.TrangThai == trangThai)
                .Include(d => d.KhachHang)
                .Include(d => d.Ban)
                .ThenInclude(b => b.KhuVucBan)
                .ToListAsync();
        }

        public async Task<bool> HasTimeConflictAsync(int banId, DateTime thoiGianDatBan)
        {
            return await _dbSet
                .AnyAsync(d => d.BanId == banId
                    && d.TrangThai == TrangThaiBanDat.DaXacNhan
                    && d.ThoiGianDatBan.Date == thoiGianDatBan.Date
                    && Math.Abs((d.ThoiGianDatBan - thoiGianDatBan).TotalHours) < 2);
        }
    }
}
