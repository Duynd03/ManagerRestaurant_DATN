using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.KhachHangRepository;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Services;
using System.Threading.Tasks;

namespace QuanLyNhaHang_DATN.Services.KhachHangService
{
    public class KhachHangService : BaseService<KhachHang>, IKhachHangService
    {
        private readonly IKhachHangRepository _khachHangRepository;

        public KhachHangService(IKhachHangRepository khachHangRepository, AppDbContext context)
            : base(khachHangRepository, context)
        {
            _khachHangRepository = khachHangRepository;
        }

        public async Task<Result<KhachHang>> TaoKhachHangAsync(string tenKhachHang, string sdt, int taiKhoanId, string diaChi, string email)
        {
            // Kiểm tra xem TaiKhoanId đã có KhachHang chưa
            var existingKhachHang = await _khachHangRepository.GetByTaiKhoanIdAsync(taiKhoanId);
            if (existingKhachHang != null)
            {
                return new Result<KhachHang>(false, "Tài khoản này đã có thông tin khách hàng.");
            }

            // Kiểm tra email đã tồn tại
            if (await _khachHangRepository.CheckExistEmailAsync(email))
            {
                return new Result<KhachHang>(false, "Email đã được sử dụng.");
            }

            var khachHang = new KhachHang
            {
                TenKhachHang = tenKhachHang,
                SDT = sdt,
                Email = email,
                DiaChi = diaChi,
                TaiKhoanId = taiKhoanId,
                NgayTao = DateTime.Now
            };

            await AddAsync(khachHang); // Sử dụng AddAsync từ BaseService

            return new Result<KhachHang>(true, "Tạo thông tin khách hàng thành công.", khachHang);
        }
    }
}