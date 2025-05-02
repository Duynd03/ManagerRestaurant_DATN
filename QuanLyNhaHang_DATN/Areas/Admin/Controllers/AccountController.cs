using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Services.NhanVienService;
using QuanLyNhaHang_DATN.Services.TaiKhoanService;
using QuanLyNhaHang_DATN.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly INhanVienService _nhanVienService;

        public AccountController(ITaiKhoanService taiKhoanService, INhanVienService nhanVienService)
        {
            _taiKhoanService = taiKhoanService;
            _nhanVienService = nhanVienService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DangNhap()
        {
            return View(new DangNhapViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                var result = await _taiKhoanService.DangNhapAsync(model);
                if (result.Success)
                {
                    // Kiểm tra vai trò: Chỉ admin, nhân viên, kế toán được truy cập khu vực Admin
                    if (result.Data.QuyenId == 1 || result.Data.QuyenId == 2 || result.Data.QuyenId == 3)
                    {
                        // Tạo danh sách Claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, result.Data.UserName),
                            new Claim("DisplayName", result.Data.QuyenId == 1 ? "Admin" : (await _nhanVienService.GetByTaiKhoanUsernameAsync(result.Data.UserName))?.TenNhanVien ?? result.Data.UserName),
                            new Claim("RoleName", result.Data.QuyenId == 1 ? "Quản lý" : (result.Data.QuyenId == 2 ? "Nhân viên" : "Kế toán")),
                            new Claim(ClaimTypes.Role, result.Data.QuyenId == 1 ? "Quản lý" : (result.Data.QuyenId == 2 ? "Nhân viên" : "Kế toán"))
                        };

                        // Tạo ClaimsIdentity
                        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

                        // Đăng nhập với Claims
                        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            new AuthenticationProperties { IsPersistent = false });

                        return Json(new { success = true, redirectUrl = Url.Action("Index", "DashBoard", new { area = "Admin" }) });
                    }
                    else
                    {
                        await _taiKhoanService.DangXuatAsync();
                        return Json(new { success = false, message = "Bạn không có quyền truy cập khu vực quản trị." });
                    }
                }

                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangXuat()
        {
            try
            {
                await _taiKhoanService.DangXuatAsync();
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme); // Đảm bảo đăng xuất scheme đúng
                return Json(new { success = true, redirectUrl = Url.Action("DangNhap", "Account", new { area = "Admin" }) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi đăng xuất: {ex.Message}" });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}