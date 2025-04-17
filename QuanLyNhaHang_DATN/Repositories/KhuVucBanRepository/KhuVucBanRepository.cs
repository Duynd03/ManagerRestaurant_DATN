using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.KhuVucBanRepository
{
    public class KhuVucBanRepository :BaseRepository<KhuVucBan>, IKhuVucBanRepository
    {
        public KhuVucBanRepository(AppDbContext context) : base(context) { 

        }
    }
}
