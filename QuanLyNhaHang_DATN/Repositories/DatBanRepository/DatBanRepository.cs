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
        public async Task<DatBan> GetDatBanByIdAsync(int id)
        {
            return await _dbSet
                .Include(db => db.KhachHang)
                .FirstOrDefaultAsync(db => db.Id == id);
        }
        public async Task<List<DatBan>> GetByTrangThaiAsync(TrangThaiBanDat trangThai)
        {
            return await _dbSet
                .Where(d => d.TrangThai == trangThai)
                .Include(d => d.KhachHang)
                .Include(d => d.DatBanBans)
                .ThenInclude(dbb => dbb.Ban)
                .ThenInclude(b => b.KhuVucBan)
                .ToListAsync();
        }


        public async Task<bool> HasTimeConflictAsync(int banId, DateTime thoiGianDatBan)
        {
            var timeRange = TimeSpan.FromHours(2);
            var startTime = thoiGianDatBan - timeRange;
            var endTime = thoiGianDatBan + timeRange;

            return await _context.DatBan_Bans
                .Include(dbb => dbb.DatBan)
                .AnyAsync(dbb =>
                    dbb.BanId == banId &&
                    dbb.DatBan.ThoiGianDatBan >= startTime &&
                    dbb.DatBan.ThoiGianDatBan <= endTime &&
                    dbb.DatBan.TrangThai != TrangThaiBanDat.DaHuy);
        }
        public async Task<DatBan> GetByIdWithDatBanBansAsync(int datBanId)
        {
            return await _dbSet
                .Include(d => d.DatBanBans).ThenInclude(dbb => dbb.Ban)
                .FirstOrDefaultAsync(d => d.Id == datBanId);
        }

        public async Task AddRangeDatBanBansAsync(IEnumerable<DatBan_Ban> datBanBans)
        {
            await _context.DatBan_Bans.AddRangeAsync(datBanBans);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRangeDatBanBansAsync(IEnumerable<DatBan_Ban> datBanBans)
        {
            _context.DatBan_Bans.RemoveRange(datBanBans);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeBanSchedulesAsync(IEnumerable<BanSchedule> banSchedules)
        {
            await _context.BanSchedules.AddRangeAsync(banSchedules);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveBanSchedulesByDatBanIdAsync(int datBanId)
        {
            var schedules = await _context.BanSchedules
                .Where(bs => bs.DatBanId == datBanId)
                .ToListAsync();
            _context.BanSchedules.RemoveRange(schedules);
            await _context.SaveChangesAsync();
        }
    }
}
