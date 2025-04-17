using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.MonAnService
{
    public interface IMonAnService : IBaseService<MonAn>
    {
        Task<IEnumerable<MonAn>> GetByDanhMucAsync(int danhMucId);
        Task<IEnumerable<MonAn>> GetAvailableAsync();
        //Task<(IEnumerable<MonAn> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, string? search = null);
        Task<(IEnumerable<MonAn> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, MonAnFilterModel filter);

    }
}
