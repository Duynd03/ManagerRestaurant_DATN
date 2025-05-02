using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.KhachHangRepository;
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

        public async Task<Result<KhachHang>> TaoKhachHangAsync(string tenKhachHang, string sdt, int? taiKhoanId, string diaChi, string email)
        {
            // Kiểm tra TaiKhoanId đã có KhachHang chưa (chỉ khi taiKhoanId không null)
            if (taiKhoanId.HasValue)
            {
                var existingKhachHang = await _khachHangRepository.GetByTaiKhoanIdAsync(taiKhoanId.Value);
                if (existingKhachHang != null)
                {
                    return new Result<KhachHang>(false, "Tài khoản này đã có thông tin khách hàng.");
                }
            }

            // Kiểm tra email đã tồn tại
            if (!string.IsNullOrEmpty(email) && await _khachHangRepository.CheckExistEmailAsync(email))
            {
                return new Result<KhachHang>(false, "Email đã được sử dụng.");
            }

            // Kiểm tra định dạng SDT
            if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d{10}$"))
            {
                return new Result<KhachHang>(false, "Số điện thoại phải có đúng 10 chữ số.");
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

            await AddAsync(khachHang);

            return new Result<KhachHang>(true, "Tạo thông tin khách hàng thành công.", khachHang);
        }
        public async Task<KhachHang> GetByTaiKhoanUsernameAsync(string username)
        {
            return await _khachHangRepository.GetByTaiKhoanUsernameAsync(username);
        }
    }
}