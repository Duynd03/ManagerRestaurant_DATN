using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.ViewModels;

namespace QuanLyNhaHang_DATN.Services.DatBanService
{
    public interface IDatBanService : IBaseService<DatBan>
    {
        Task<Result<DatBan>> CreateDatBanAsync(DatBanViewModel viewModel, string username);
        Task<List<Ban>> GetAvailableBansAsync();
        Task<Ban> GetBanByIdAsync(int banId);
        Task<List<DatBan>> GetByTrangThaiAsync(TrangThaiBanDat trangThai);
        Task<bool> HasTimeConflictAsync(int banId, DateTime thoiGianDatBan);
        Task UpdateBanAsync(Ban ban);
        Task<(IEnumerable<DatBan> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, DatBanFilterModel filter);
        //Task<Result<DatBan>> XepBanAsync(int datBanId, int[] banIds, int? nhanVienId);
        Task AddDatBanBanAsync(DatBan_Ban datBanBan); // Thêm khai báo
        Task<Result<DatBan>> XepBanAsync(int datBanId, List<int> banIds, int? nhanVienId);
        Task<List<string>> GetBanGhepAsync(int datBanId);
        Task<DatBan> GetByIdWithKhachHangAsync(int id);
       
        Task<Result<DatBan>> ChuyenBanAsync(int datBanId, List<int> banIds, int nhanVienId);
        Task<Result<DatBan>> HuyBanAsync(int datBanId, int nhanVienId);
        // 
        IQueryable<BanSchedule> GetBanSchedules();
        Task UpdateBanSauThanhToanAsync(int datBanId, DateTime ThoiGianKetThuc);
        Task UpdateBanGoiMonAsync(int datBanId);
        //
        Task<List<int>> GetCurrentBanIdsAsync(int datBanId);
        Task<TrangThaiBanViewModel> GetTableStatusAsync(int banId, DateTime currentTime);
        //Task<KhaDungBanViewModel> CheckTableAvailabilityAsync(int banId, DateTime bookingTime, DateTime bookingEndTime);
        Task<KhaDungBanViewModel> CheckTableAvailabilityAsync(int banId, DateTime bookingTime, DateTime bookingEndTime, int? currentDatBanId = null);
    }

}
