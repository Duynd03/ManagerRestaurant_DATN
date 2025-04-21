using Microsoft.AspNetCore.Identity;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.TaiKhoanRepository;
using QuanLyNhaHang_DATN.ViewModels;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Services;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using System.Threading.Tasks;

namespace QuanLyNhaHang_DATN.Services.TaiKhoanService
{
    public class TaiKhoanService : BaseService<TaiKhoan>, ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;
        private readonly IKhachHangService _khachHangService;

        public TaiKhoanService(ITaiKhoanRepository taiKhoanRepository, IKhachHangService khachHangService, AppDbContext context)
            : base(taiKhoanRepository, context)
        {
            _taiKhoanRepository = taiKhoanRepository;
            _khachHangService = khachHangService;
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

            var hasher = new PasswordHasher<TaiKhoan>();

            var taiKhoan = new TaiKhoan
            {
                TenTaiKhoan = model.TenTaiKhoan,
                MatKhau = "", // Sẽ được hashed sau
                QuyenId = 4, // Khách hàng
                TrangThai = TrangThaiTaiKhoan.DangHoatDong,
                NgayTao = DateTime.Now
            };

            taiKhoan.MatKhau = hasher.HashPassword(taiKhoan, model.MatKhau);
            await AddAsync(taiKhoan); // Sử dụng phương thức AddAsync từ BaseService

            // Tạo bản ghi KhachHang
            var khachHangResult = await _khachHangService.TaoKhachHangAsync(
                model.TenKhachHang,
                model.SDT,
                taiKhoan.Id,
                model.DiaChi,
                model.Email
            );

            if (!khachHangResult.Success)
            {
                // Nếu không tạo được KhachHang, xóa TaiKhoan vừa tạo
                await DeleteAsync(taiKhoan.Id);
                return new Result<TaiKhoan>(false, "Tạo tài khoản thất bại: " + khachHangResult.Message);
            }

            return new Result<TaiKhoan>(true, "Đăng ký thành công.", taiKhoan);
        }

        public async Task<Result<TaiKhoan>> DangNhapAsync(DangNhapViewModel model)
        {
            var taiKhoan = await _taiKhoanRepository.GetByUsernameAsync(model.TenTaiKhoan);
            if (taiKhoan == null)
            {
                return new Result<TaiKhoan>(false, "Tên tài khoản không tồn tại.");
            }

            var hasher = new PasswordHasher<TaiKhoan>();
            var result = hasher.VerifyHashedPassword(taiKhoan, taiKhoan.MatKhau, model.MatKhau);

            if (result != PasswordVerificationResult.Success)
            {
                return new Result<TaiKhoan>(false, "Mật khẩu không đúng.");
            }

            return new Result<TaiKhoan>(true, "Đăng nhập thành công.", taiKhoan);
        }
    }

}