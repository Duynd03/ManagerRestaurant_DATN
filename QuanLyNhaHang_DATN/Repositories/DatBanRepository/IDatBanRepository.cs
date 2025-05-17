using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.DatBanRepository
{
    public interface IDatBanRepository : IBaseRepository<DatBan>
    {
        Task<DatBan> CreateAsync(DatBan datBan);
        Task<List<DatBan>> GetByTrangThaiAsync(TrangThaiBanDat trangThai);
        Task<bool> HasTimeConflictAsync(int banId, DateTime thoiGianDatBan);
        Task<DatBan> GetByIdWithDatBanBansAsync(int datBanId); // Lấy DatBan kèm DatBanBans
        Task AddRangeDatBanBansAsync(IEnumerable<DatBan_Ban> datBanBans); // Thêm nhiều DatBan_Ban
        Task RemoveRangeDatBanBansAsync(IEnumerable<DatBan_Ban> datBanBans); // Xóa nhiều DatBan_Ban
        Task AddRangeBanSchedulesAsync(IEnumerable<BanSchedule> banSchedules); // Thêm nhiều BanSchedule
        Task RemoveBanSchedulesByDatBanIdAsync(int datBanId); // Xóa BanSchedule theo DatBanId
    }
}
