using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.GoiMonRepository;

namespace QuanLyNhaHang_DATN.Services.GoiMonService
{
    public class GoiMonService : BaseService<GoiMon>, IGoiMonService
    {
        private readonly IGoiMonRepository _goiMonRepository;

        public GoiMonService(IGoiMonRepository repository, AppDbContext context)
            : base(repository, context)
        {
            _goiMonRepository = repository;
        }

        public async Task<IEnumerable<GoiMon>> GetByDatBanIdAsync(int datBanId)
        {
            return await _goiMonRepository.GetByDatBanIdAsync(datBanId);
        }

        public async Task<(IEnumerable<GoiMon> Items, int TotalCount)> GetPagedByDatBanIdAsync(int datBanId, int pageIndex, int pageSize)
        {
            var query = _goiMonRepository.Query().Where(gm => gm.DatBanId == datBanId);
            var total = await query.CountAsync();
            var items = await _goiMonRepository.GetPagedByDatBanIdAsync(datBanId, pageIndex, pageSize);
            return (items, total);
        }

        public async Task SaveGoiMonListAsync(int datBanId, List<GoiMon> goiMonList)
        {
            if (goiMonList == null || !goiMonList.Any())
            {
                throw new ArgumentException("Danh sách gọi món không được rỗng.");
            }
            var datBanExists = await _context.DatBans.AnyAsync(d => d.Id == datBanId);
            if (!datBanExists)
            {
                throw new InvalidOperationException($"Đặt bàn với ID = {datBanId} không tồn tại.");
            }
            // Xóa các món đã gọi trước đó của đơn đặt bàn
            await DeleteGoiMonByDatBanIdAsync(datBanId);

            // Lưu danh sách món mới
            foreach (var goiMon in goiMonList)
            {
                goiMon.DatBanId = datBanId;
                goiMon.ThoiGianGoiMon = DateTime.Now;
                await _goiMonRepository.AddAsync(goiMon); 
            }
            await _goiMonRepository.SaveChangesAsync(); 
        }

        public async Task<decimal> CalculateTongTienAsync(int datBanId)
        {
            return await _goiMonRepository.CalculateTongTienByDatBanIdAsync(datBanId);
        }

        public async Task<bool> DeleteGoiMonByDatBanIdAsync(int datBanId)
        {
            var goiMons = await _goiMonRepository.GetByDatBanIdAsync(datBanId);
            if (!goiMons.Any()) return true;

            foreach (var goiMon in goiMons)
            {
                await _goiMonRepository.DeleteAsync(goiMon); 
            }
            await _goiMonRepository.SaveChangesAsync(); 
            return true;
        }
    }
}