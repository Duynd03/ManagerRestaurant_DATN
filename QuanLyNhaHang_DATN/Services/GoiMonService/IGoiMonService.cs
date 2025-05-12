using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.GoiMonService
{
    public interface IGoiMonService : IBaseService<GoiMon>
    {
        Task<IEnumerable<GoiMon>> GetByDatBanIdAsync(int datBanId);
        Task<(IEnumerable<GoiMon> Items, int TotalCount)> GetPagedByDatBanIdAsync(int datBanId, int pageIndex, int pageSize);
        Task SaveGoiMonListAsync(int datBanId, List<GoiMon> goiMonList);
        Task<decimal> CalculateTongTienAsync(int datBanId);
        Task<bool> DeleteGoiMonByDatBanIdAsync(int datBanId); // Xóa tất cả gọi món của một đơn đặt bàn
    }
}
