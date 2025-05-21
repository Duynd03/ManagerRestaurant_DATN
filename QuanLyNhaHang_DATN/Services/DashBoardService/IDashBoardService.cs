using QuanLyNhaHang_DATN.ViewModels;

namespace QuanLyNhaHang_DATN.Services.DashBoardService
{
    public interface IDashBoardService
    {
        Task<DashBoardViewModel> GetDashBoardDataAsync(string filterType = "daily", string fromDate = null, string toDate = null, int? month = null, int? year = null, int? compareYear = null);
    }
}
