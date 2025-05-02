using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.BanService
{
    public interface IBanService : IBaseService<Ban>
    {
        Task<IEnumerable<Ban>> GetByKhuVucBanAsync(int khuVucBanId);
        Task<List<Ban>> GetAvailableBansAsync();

        Task<(IEnumerable<Ban> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, BanFilterModel filter);
    }
}
