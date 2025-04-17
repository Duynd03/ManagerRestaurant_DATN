using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.DanhMucRepository;

namespace QuanLyNhaHang_DATN.Repositories.MonAnRepository
{
    public class MonAnRepository : BaseRepository<MonAn>, IMonAnRepository
    {
        public MonAnRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<MonAn>> GetByDanhMucAsync(int danhMucId)
        {
            return await _dbSet.Where(m => m.DanhMucId == danhMucId).ToListAsync();
        }

        public async Task<IEnumerable<MonAn>> GetAvailableAsync()
        {
            return await _dbSet.Where(m => m.TrangThai == TrangThaiMonAn.CoSan).ToListAsync();
        }

    }
}
