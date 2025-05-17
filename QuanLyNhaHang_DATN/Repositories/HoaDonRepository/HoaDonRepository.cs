using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories;

namespace QuanLyNhaHang_DATN.Repositories.HoaDonRepository
{
    public class HoaDonRepository : BaseRepository<HoaDon>, IHoaDonRepository
    {
        public HoaDonRepository(AppDbContext context) : base(context) { }

        public async Task<HoaDon> GetByDatBanIdAsync(int datBanId)
        {
            return await _context.HoaDons
                .FirstOrDefaultAsync(hd => hd.DatBanId == datBanId && hd.TrangThai == TrangThaiHoaDon.ChuaThanhToan);
        }

        public async Task<IEnumerable<HoaDon>> GetByTrangThaiAsync(TrangThaiHoaDon trangThai)
        {
            return await _context.HoaDons
                .Where(hd => hd.TrangThai == trangThai)
                .ToListAsync();
        }
    }
}