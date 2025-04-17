using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.DanhMucRepository
{
    public interface IDanhMucRepository : IBaseRepository<DanhMuc>
    {
        IQueryable<DanhMuc> Search(string name);
    }
}
