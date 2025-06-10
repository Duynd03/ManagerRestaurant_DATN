using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.GoiMonService
{
    public interface IGoiMonService : IBaseService<GoiMon>
    {
        Task<IEnumerable<GoiMon>> GetByDatBanIdAsync(int datBanId);
        Task<(IEnumerable<GoiMon> Items, int TotalCount)> GetPagedByDatBanIdAsync(int datBanId, int pageIndex, int pageSize);
        Task SaveGoiMonListAsync(int datBanId, List<GoiMon> goiMonList, int lanGoiMon);
        Task<decimal> CalculateTongTienAsync(int datBanId);
        Task<bool> DeleteGoiMonByDatBanIdAsync(int datBanId);
        Task<int> GetMaxLanGoiMonAsync(int datBanId); // Lấy lần gọi mới nhất
        Task<IEnumerable<GoiMon>> GetByDatBanIdAndLanGoiMonAsync(int datBanId, int lanGoiMon); // Lấy món theo lần gọi
    }
}
