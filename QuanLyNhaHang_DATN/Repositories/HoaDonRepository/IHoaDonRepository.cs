using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.HoaDonRepository
{
    public interface IHoaDonRepository : IBaseRepository<HoaDon>
    {
        Task<HoaDon> GetByDatBanIdAsync(int datBanId);
        Task<IEnumerable<HoaDon>> GetByTrangThaiAsync(TrangThaiHoaDon trangThai);
    }
}
