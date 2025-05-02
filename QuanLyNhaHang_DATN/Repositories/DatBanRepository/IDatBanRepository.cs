using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.DatBanRepository
{
    public interface IDatBanRepository : IBaseRepository<DatBan>
    {
        Task<DatBan> CreateAsync(DatBan datBan);
        Task<List<DatBan>> GetByTrangThaiAsync(TrangThaiBanDat trangThai);
        Task<bool> HasTimeConflictAsync(int banId, DateTime thoiGianDatBan);
    }
}
