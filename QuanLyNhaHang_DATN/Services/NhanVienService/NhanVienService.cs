using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.NhanVienRepository;

namespace QuanLyNhaHang_DATN.Services.NhanVienService
{
    public class NhanVienService : BaseService<NhanVien>, INhanVienService
    {
        private readonly INhanVienRepository _nhanVienRepository;

        public NhanVienService(INhanVienRepository nhanVienRepository, AppDbContext context)
            : base(nhanVienRepository, context)
        {
            _nhanVienRepository = nhanVienRepository;
        }

        // Lấy thông tin nhân viên dựa trên username
        public async Task<NhanVien> GetByTaiKhoanUsernameAsync(string username)
        {
            return await _nhanVienRepository.GetByTaiKhoanUsernameAsync(username);
        }

        // Tạo thông tin nhân viên mới
        public async Task<Result<NhanVien>> TaoNhanVienAsync(string tenNhanVien, string sdt, DateTime? ngaySinh, string diaChi, int taiKhoanId)
        {
            // Kiểm tra xem tài khoản đã có thông tin nhân viên chưa
            var existingNhanVien = await _nhanVienRepository.GetByTaiKhoanIdAsync(taiKhoanId);
            if (existingNhanVien != null)
            {
                return new Result<NhanVien>(false, "Tài khoản này đã có thông tin nhân viên.");
            }

            // Kiểm tra số điện thoại đã tồn tại chưa
            if (await _nhanVienRepository.CheckExistSdtAsync(sdt))
            {
                return new Result<NhanVien>(false, "Số điện thoại đã được sử dụng.");
            }

            // Kiểm tra định dạng số điện thoại (10 chữ số)
            if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d{10}$"))
            {
                return new Result<NhanVien>(false, "Số điện thoại phải có đúng 10 chữ số.");
            }

            // Tạo đối tượng nhân viên mới
            var nhanVien = new NhanVien
            {
                TenNhanVien = tenNhanVien,
                Sdt = sdt,
                NgaySinh = ngaySinh,
                DiaChi = diaChi,
                TaiKhoanId = taiKhoanId,
                NgayTao = DateTime.Now
            };

            // Lưu vào database
            await AddAsync(nhanVien);

            return new Result<NhanVien>(true, "Tạo thông tin nhân viên thành công.", nhanVien);
        }
    }
}
