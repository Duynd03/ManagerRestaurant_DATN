using Microsoft.AspNetCore.Identity;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.NhanVienRepository;
using QuanLyNhaHang_DATN.Repositories.TaiKhoanRepository;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using QuanLyNhaHang_DATN.ViewModels;
using System.Threading.Tasks;

namespace QuanLyNhaHang_DATN.Services.TaiKhoanService
{
    public class TaiKhoanService : BaseService<TaiKhoan>, ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly IKhachHangService _khachHangService;
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;

        public TaiKhoanService(
            ITaiKhoanRepository taiKhoanRepository,
            IKhachHangService khachHangService,
            INhanVienRepository nhanVienRepository,
            AppDbContext context,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager)
            : base(taiKhoanRepository, context)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _nhanVienRepository = nhanVienRepository;
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
            // Bước 1: Kiểm tra tên đăng nhập đã tồn tại chưa
            if (await _taiKhoanRepository.CheckExistUsernameAsync(username))
            {
                return new Result<TaiKhoan>(false, "Tên tài khoản đã tồn tại.");
            }

            // Bước 2: Tạo tài khoản mới
            var taiKhoan = new TaiKhoan
            {
                UserName = username,
                QuyenId = quyenId,
                TrangThai = TrangThaiTaiKhoan.DangHoatDong,
                NgayTao = DateTime.Now
            };

            var createResult = await _taiKhoanRepository.CreateAsync(taiKhoan, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return new Result<TaiKhoan>(false, $"Tạo tài khoản thất bại: {errors}");
            }

            // Bước 3: Gán vai trò cho tài khoản
            string roleName = quyenId switch
            {
                1 => "Quản lý",
                2 => "Nhân viên phục vụ",
                3 => "Kế toán",
                4 => "Khách hàng",
                _ => throw new ArgumentException("QuyenId không hợp lệ.")
            };

            var roleResult = await _userManager.AddToRoleAsync(taiKhoan, roleName);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(taiKhoan);
                var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return new Result<TaiKhoan>(false, $"Gán vai trò thất bại: {roleErrors}");
            }

            // Bước 4: Nếu là nhân viên hoặc kế toán, tạo thông tin nhân viên
            if (quyenId == 2 || quyenId == 3)
            {
                // Kiểm tra xem có đủ thông tin để tạo nhân viên không
                if (string.IsNullOrEmpty(tenNhanVien) || string.IsNullOrEmpty(sdt))
                {
                    await _userManager.DeleteAsync(taiKhoan);
                    return new Result<TaiKhoan>(false, "Tên nhân viên và số điện thoại không được để trống.");
                }

                // Kiểm tra xem tài khoản đã có thông tin nhân viên chưa
                var existingNhanVien = await _nhanVienRepository.GetByTaiKhoanIdAsync(taiKhoan.Id);
                if (existingNhanVien != null)
                {
                    await _userManager.DeleteAsync(taiKhoan);
                    return new Result<TaiKhoan>(false, "Tài khoản này đã có thông tin nhân viên.");
                }

                // Kiểm tra số điện thoại đã tồn tại chưa
                if (await _nhanVienRepository.CheckExistSdtAsync(sdt))
                {
                    await _userManager.DeleteAsync(taiKhoan);
                    return new Result<TaiKhoan>(false, "Số điện thoại đã được sử dụng.");
                }

                // Kiểm tra định dạng số điện thoại (10 chữ số)
                if (!System.Text.RegularExpressions.Regex.IsMatch(sdt, @"^\d{10}$"))
                {
                    await _userManager.DeleteAsync(taiKhoan);
                    return new Result<TaiKhoan>(false, "Số điện thoại phải có đúng 10 chữ số.");
                }

                // Tạo đối tượng nhân viên mới
                var nhanVien = new NhanVien
                {
                    TenNhanVien = tenNhanVien,
                    Sdt = sdt,
                    NgaySinh = ngaySinh,
                    DiaChi = diaChi,
                    TaiKhoanId = taiKhoan.Id,
                    NgayTao = DateTime.Now
                };

                // Lưu nhân viên vào database
                await _context.NhanViens.AddAsync(nhanVien);
                await _context.SaveChangesAsync();
            }

            // Bước 5: Trả về kết quả thành công
            return new Result<TaiKhoan>(true, "Tạo tài khoản thành công.", taiKhoan);
        }
    }
}