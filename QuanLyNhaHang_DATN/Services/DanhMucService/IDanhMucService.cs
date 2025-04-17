
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.DanhMucService
{
    public interface IDanhMucService : IBaseService<DanhMuc>
    {
        Task<(IEnumerable<DanhMuc> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, DanhMucFilterModel filter);
    }

}
