using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Services.HoaDonService
{
    public interface IHoaDonService : IBaseService<HoaDon>
    {
        Task<HoaDon> CreateHoaDonAsync(int datBanId);
        Task ConfirmPaymentAsync(int id, PhuongThucThanhToan phuongThuc,int nhanVienId);
        Task<HoaDon> GetHoaDonChuaThanhToanByDatBanIdAsync(int datBanId);
        Task<IEnumerable<HoaDon>> GetLichSuHoaDonAsync();
        Task<HoaDon> GetByIdWithDetailsAsync(int id);
        Task<(IEnumerable<HoaDon> Items, int TotalCount)> GetPagedHoaDonsAsync(int pageIndex, int pageSize, HoaDonFilterModel filter);
        string GenerateMaHoaDon(int datBanId); // sinh mã hóa đơn
    }
}
