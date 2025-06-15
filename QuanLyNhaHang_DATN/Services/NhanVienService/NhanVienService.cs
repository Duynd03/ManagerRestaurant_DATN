using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
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

            // Kiểm tra định dạng số điện thoại
            if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d{10}$"))
            {
                return new Result<NhanVien>(false, "Số điện thoại phải có đúng 10 chữ số.");
            }

            var nhanVien = new NhanVien
            {
                TenNhanVien = tenNhanVien,
                Sdt = sdt,
                NgaySinh = ngaySinh,
                DiaChi = diaChi,
                TaiKhoanId = taiKhoanId,
                NgayTao = DateTime.Now
            };

            await AddAsync(nhanVien); // Sử dụng BaseService để lưu
            return new Result<NhanVien>(true, "Tạo thông tin nhân viên thành công.", nhanVien);
        }

        public async Task<(IEnumerable<NhanVien> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, NhanVienFilterModel filter)
        {
            IQueryable<NhanVien> query = _context.NhanViens
            .Include(nv => nv.TaiKhoan)
            .ThenInclude(tk => tk.Quyen)
            .Where(nv => nv.TaiKhoan != null
            && (nv.TaiKhoan.QuyenId == 1 || nv.TaiKhoan.QuyenId == 2 || nv.TaiKhoan.QuyenId == 3)
            && nv.TaiKhoan.TrangThai == TrangThaiTaiKhoan.DangHoatDong);

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TenNhanVien))
                {
                    query = query.Where(nv => nv.TenNhanVien.Contains(filter.TenNhanVien));
                }

                if (filter.QuyenId.HasValue)
                {
                    query = query.Where(nv => nv.TaiKhoan.QuyenId == filter.QuyenId.Value);
                }

            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(nv => nv.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
        public async Task<NhanVien> GetByTaiKhoanIdAsync(int taiKhoanId)
        {
            return await _nhanVienRepository.GetByTaiKhoanIdAsync(taiKhoanId);
        }
        public async Task<NhanVien> GetByIdAsync(int id)
        {
            return await _context.NhanViens
                .Include(nv => nv.TaiKhoan)
                .ThenInclude(tk => tk.Quyen)
                .FirstOrDefaultAsync(nv => nv.Id == id);
        }
    }
}
