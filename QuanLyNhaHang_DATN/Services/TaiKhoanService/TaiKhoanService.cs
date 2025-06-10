using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.NhanVienRepository;
using QuanLyNhaHang_DATN.Repositories.TaiKhoanRepository;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using QuanLyNhaHang_DATN.Services.NhanVienService;
using QuanLyNhaHang_DATN.ViewModels;
using System.Threading.Tasks;

namespace QuanLyNhaHang_DATN.Services.TaiKhoanService
{
    public class TaiKhoanService : BaseService<TaiKhoan>, ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly IKhachHangService _khachHangService;
        private readonly INhanVienService _nhanVienService;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;

        public TaiKhoanService(
            ITaiKhoanRepository taiKhoanRepository,
            IKhachHangService khachHangService,
           INhanVienService nhanVienService,
            AppDbContext context,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager)
            : base(taiKhoanRepository, context)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _nhanVienService = nhanVienService;
            _khachHangService = khachHangService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Result<TaiKhoan>> DangKyAsync(DangKyViewModel model)
        {
            // Kiểm tra tên tài khoản đã tồn tại
            if (await _taiKhoanRepository.CheckExistUsernameAsync(model.TenTaiKhoan))
            {
                return new Result<TaiKhoan>(false, "Tên tài khoản đã tồn tại.");
            }

            // Kiểm tra mật khẩu xác nhận
            if (model.MatKhau != model.NhapLaiMatKhau)
            {
                return new Result<TaiKhoan>(false, "Mật khẩu xác nhận không khớp.");
            }

            var taiKhoan = new TaiKhoan
            {
                UserName = model.TenTaiKhoan,
                QuyenId = 4, // Khách hàng
                TrangThai = TrangThaiTaiKhoan.DangHoatDong,
                NgayTao = DateTime.Now
            };

            var result = await _taiKhoanRepository.CreateAsync(taiKhoan, model.MatKhau);
            if (result.Succeeded)
            {
                var khachHangResult = await _khachHangService.TaoKhachHangAsync(
                    model.TenKhachHang,
                    model.SDT,
                    taiKhoan.Id,
                    model.DiaChi,
                    model.Email
                );

                if (!khachHangResult.Success)
                {
                    await _userManager.DeleteAsync(taiKhoan);
                    return new Result<TaiKhoan>(false, "Tạo tài khoản thất bại: " + khachHangResult.Message);
                }

                return new Result<TaiKhoan>(true, "Đăng ký thành công.", taiKhoan);
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new Result<TaiKhoan>(false, $"Đăng ký thất bại: {errors}");
        }

        public async Task<Result<TaiKhoan>> DangNhapAsync(DangNhapViewModel model)
        {
            var taiKhoan = await _taiKhoanRepository.GetByUsernameAsync(model.TenTaiKhoan);
            if (taiKhoan == null)
            {
                return new Result<TaiKhoan>(false, "Tên tài khoản không tồn tại.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.TenTaiKhoan,
                model.MatKhau,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                if (taiKhoan.TrangThai == TrangThaiTaiKhoan.DaKhoa)
                {
                    await _signInManager.SignOutAsync();
                    return new Result<TaiKhoan>(false, "Tài khoản của bạn đã bị khóa.");
                }

                return new Result<TaiKhoan>(true, "Đăng nhập thành công.", taiKhoan);
            }

            return new Result<TaiKhoan>(false, "Mật khẩu không đúng.");
        }
        public async Task DangXuatAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Result<TaiKhoan>> CreateUserAsync(
            string username,
            string password,
            int quyenId,
            string tenNhanVien,
            string sdt,
            DateTime? ngaySinh,
            string diaChi)
        {
            if (await _taiKhoanRepository.CheckExistUsernameAsync(username))
            {
                return new Result<TaiKhoan>(false, "Tên tài khoản đã tồn tại.");
            }

            var taiKhoan = new TaiKhoan
            {
                UserName = username,
                QuyenId = quyenId,
                TrangThai = TrangThaiTaiKhoan.DangHoatDong,
                NgayTao = DateTime.Now
            };

            var createResult = await _userManager.CreateAsync(taiKhoan, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return new Result<TaiKhoan>(false, $"Tạo tài khoản thất bại: {errors}");
            }

            string roleName = quyenId switch
            {
                1 => "Admin",
                2 => "NhanVien",
                3 => "KeToan",
                4 => "KhachHang",
                _ => throw new ArgumentException("QuyenId không hợp lệ.")
            };

            var roleResult = await _userManager.AddToRoleAsync(taiKhoan, roleName);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(taiKhoan);
                var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return new Result<TaiKhoan>(false, $"Gán vai trò thất bại: {roleErrors}");
            }

            if (quyenId == 2 || quyenId == 3)
            {
                if (string.IsNullOrEmpty(tenNhanVien) || string.IsNullOrEmpty(sdt))
                {
                    await _userManager.DeleteAsync(taiKhoan);
                    return new Result<TaiKhoan>(false, "Tên nhân viên và số điện thoại không được để trống.");
                }

                var nhanVienResult = await _nhanVienService.TaoNhanVienAsync(tenNhanVien, sdt, ngaySinh, diaChi, taiKhoan.Id);
                if (!nhanVienResult.Success)
                {
                    await _userManager.DeleteAsync(taiKhoan);
                    return new Result<TaiKhoan>(false, nhanVienResult.Message);
                }
            }

            return new Result<TaiKhoan>(true, "Tạo tài khoản thành công.", taiKhoan);
        }

        public async Task<Result<TaiKhoan>> UpdateUserAsync(int nhanVienId, string tenNhanVien, string sdt, DateTime? ngaySinh, string diaChi, int quyenId)
        {
           
            // Lấy nhân viên theo Id
            var nhanVien = await _nhanVienService.GetByIdAsync(nhanVienId);
            if (nhanVien == null)
            {
                return new Result<TaiKhoan>(false, "Nhân viên không tồn tại.");
            }

            // Cập nhật thông tin nhân viên
            nhanVien.TenNhanVien = tenNhanVien;
            nhanVien.Sdt = sdt;
            nhanVien.NgaySinh = ngaySinh;
            nhanVien.DiaChi = diaChi;

            // Kiểm tra số điện thoại đã tồn tại chưa (trừ chính nhân viên đang cập nhật)
            var existingNhanVienWithSdt = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.Sdt == sdt && nv.Id != nhanVienId);
            if (existingNhanVienWithSdt != null)
            {
                return new Result<TaiKhoan>(false, "Số điện thoại đã được sử dụng.");
            }

            // Kiểm tra định dạng số điện thoại
            if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d{10}$"))
            {
                return new Result<TaiKhoan>(false, "Số điện thoại phải có đúng 10 chữ số.");
            }

            // Lấy tài khoản
            var taiKhoan = await _userManager.FindByIdAsync(nhanVien.TaiKhoanId.ToString());
            if (taiKhoan == null)
            {
                return new Result<TaiKhoan>(false, "Tài khoản không tồn tại.");
            }

            // Cập nhật QuyenId
            taiKhoan.QuyenId = quyenId;

            // Ánh xạ QuyenId với vai trò
            string roleName = quyenId switch
            {
                1 => "Admin",
                2 => "NhanVien",
                3 => "KeToan",
                4 => "KhachHang",
                _ => throw new ArgumentException("QuyenId không hợp lệ.")
            };

            // Xóa vai trò cũ và gán vai trò mới
            var currentRoles = await _userManager.GetRolesAsync(taiKhoan);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(taiKhoan, currentRoles);
            }

            var roleResult = await _userManager.AddToRoleAsync(taiKhoan, roleName);
            if (!roleResult.Succeeded)
            {
                return new Result<TaiKhoan>(false, $"Gán vai trò thất bại: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }

            // Lưu thay đổi
            await _nhanVienService.UpdateAsync(nhanVien);
            await _userManager.UpdateAsync(taiKhoan);

            return new Result<TaiKhoan>(true, "Cập nhật nhân viên thành công.", taiKhoan);
        }

        public async Task<List<Quyen>> GetAllQuyenAsync()
        {
            return await _context.Quyens
                .Where(q => q.Id == 1 || q.Id == 2 || q.Id == 3) //lấy quyền Quản lý, Nhân viên phục vụ, Kế toán
                .ToListAsync();
        }
    }
}