using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.MonAnRepository
{
    public interface IMonAnRepository : IBaseRepository<MonAn>
    {
        Task<IEnumerable<MonAn>> GetByDanhMucAsync(int danhMucId);
        Task<IEnumerable<MonAn>> GetAvailableAsync();
    }
}
