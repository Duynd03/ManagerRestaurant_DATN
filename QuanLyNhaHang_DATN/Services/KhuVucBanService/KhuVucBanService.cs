using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.DanhMucRepository;
using QuanLyNhaHang_DATN.Repositories.KhuVucBanRepository;

namespace QuanLyNhaHang_DATN.Services.KhuVucBanService
{
    public class KhuVucBanService : BaseService<KhuVucBan>, IKhuVucBanService
    {
        private readonly IKhuVucBanRepository _khuVucBanRepository;
        public KhuVucBanService(IKhuVucBanRepository repository, AppDbContext context) : base(repository, context)
        {
            _khuVucBanRepository = repository;
        }
    }
}
