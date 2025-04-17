using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.BanRepository
{
    public interface IBanRepository : IBaseRepository<Ban>
    {
        Task<IEnumerable<Ban>> GetByKhuVucBanAsync(int KhuVucBanId);
        //Task<IEnumerable<Ban>> GetAvailableAsync();
    }
}
