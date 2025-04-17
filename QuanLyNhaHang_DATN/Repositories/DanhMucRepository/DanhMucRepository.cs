using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.DanhMucRepository
{
    public class DanhMucRepository : BaseRepository<DanhMuc>, IDanhMucRepository
    {
        public DanhMucRepository(AppDbContext context) : base(context) { }

        public IQueryable<DanhMuc> Search(string name)
        {
            return _dbSet.Where(dm => dm.TenDanhMuc.Contains(name));
        }
    }

}
