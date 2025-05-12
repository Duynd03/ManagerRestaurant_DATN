
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.GoiMonRepository
{
    public interface IGoiMonRepository : IBaseRepository<GoiMon>
    {
        Task<IEnumerable<GoiMon>> GetByDatBanIdAsync(int datBanId);
        Task<IEnumerable<GoiMon>> GetPagedByDatBanIdAsync(int datBanId, int pageIndex, int pageSize);
        Task<decimal> CalculateTongTienByDatBanIdAsync(int datBanId);
    }
}