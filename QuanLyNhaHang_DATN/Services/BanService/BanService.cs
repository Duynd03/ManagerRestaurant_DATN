using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.BanRepository;
using QuanLyNhaHang_DATN.Repositories.KhuVucBanRepository;


namespace QuanLyNhaHang_DATN.Services.BanService
{
    public class BanService :BaseService<Ban>, IBanService
    {
        private readonly IBanRepository _banRepository;
        private readonly IKhuVucBanRepository _khuVucBanRepository;
        public BanService(IBanRepository repository, IKhuVucBanRepository khuVucBanRepository, AppDbContext context) : base(repository, context)
        {
            _banRepository = repository;
            _khuVucBanRepository = khuVucBanRepository;
        }
        public async Task<IEnumerable<Ban>> GetByKhuVucBanAsync(int khuVucBanId)
        {
            return await _banRepository.GetByKhuVucBanAsync(khuVucBanId);
        }
        public async Task<List<Ban>> GetAvailableBansAsync()
        {
            var bans = await _banRepository.GetAvailableAsync();
            return bans.ToList();
        }

        public async Task<(IEnumerable<Ban> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, BanFilterModel filter)
        {
            IQueryable<Ban> query = _banRepository.Query().Include(m => m.KhuVucBan);

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TenBan))
                {
                    query = query.Where(m => m.TenBan.Contains(filter.TenBan));
                }

                if (filter.KhuVucBanId.HasValue)
                {
                    query = query.Where(m => m.KhuVucBanId == filter.KhuVucBanId);
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(m => (int)m.TrangThai == filter.TrangThai.Value);
                }
            }

            var total = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
 