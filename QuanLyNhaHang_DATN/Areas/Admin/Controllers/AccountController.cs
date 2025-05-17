using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Services.NhanVienService;
using QuanLyNhaHang_DATN.Services.TaiKhoanService;
using QuanLyNhaHang_DATN.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly INhanVienService _nhanVienService;
        private readonly UserManager<TaiKhoan> _userManager;
        private readonly SignInManager<TaiKhoan> _signInManager;

        public AccountController(
            ITaiKhoanService taiKhoanService,
            INhanVienService nhanVienService,
            UserManager<TaiKhoan> userManager,
            SignInManager<TaiKhoan> signInManager)
        {
            _taiKhoanService = taiKhoanService;
            _nhanVienService = nhanVienService;
            _userManager = userManager;
            _signInManager = signInManager;
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
                    var user = await _userManager.FindByNameAsync(model.TenTaiKhoan);
                    if (user == null)
                    {
                        return Json(new { success = false, message = "Người dùng không tồn tại trong hệ thống Identity." });
                    }

                    // Kiểm tra vai trò: Chỉ cho phép Admin, Nhân viên, Kế toán (QuyenId = 1, 2, 3)
                    if (user.QuyenId != 1 && user.QuyenId != 2 && user.QuyenId != 3)
                    {
                        await _signInManager.SignOutAsync();
                        return Json(new { success = false, message = "Bạn không có quyền truy cập khu vực quản trị." });
                    }

                    // Xóa các claims cũ nếu có
                    var existingClaims = await _userManager.GetClaimsAsync(user);
                    if (existingClaims.Any())
                    {
                        await _userManager.RemoveClaimsAsync(user, existingClaims);
                    }

                    // Thêm thông tin bổ sung vào Claims
                    string displayName = user.UserName;
                    string nhanVienId = null;
                    string tenNhanVien = null;
                    string roleName = "Không xác định";

                    // Xác định role và thông tin nhân viên
                    if (result.Data.QuyenId == 1 || result.Data.QuyenId == 2 || result.Data.QuyenId == 3)
                    {
                        var nhanVien = await _nhanVienService.GetByTaiKhoanUsernameAsync(user.UserName);
                        if (nhanVien != null)
                        {
                            nhanVienId = nhanVien.Id.ToString();
                            tenNhanVien = nhanVien.TenNhanVien;
                            displayName = tenNhanVien;

                            roleName = result.Data.QuyenId switch
                            {
                                1 => "Admin",
                                2 => "NhanVien",
                                3 => "KeToan",
                                _ => "Không xác định"
                            };
                        }
                        else
                        {
                            return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên cho tài khoản này." });
                        }
                    }

                    // Tạo danh sách claims
                    var claims = new List<Claim>
            {
                new Claim("DisplayName", displayName),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("RoleName", roleName)
            };

                    if (!string.IsNullOrEmpty(nhanVienId))
                    {
                        claims.Add(new Claim("NhanVienId", nhanVienId));
                    }
                    if (!string.IsNullOrEmpty(tenNhanVien))
                    {
                        claims.Add(new Claim("TenNhanVien", tenNhanVien));
                    }

                    // Thêm claims vào user
                    await _userManager.AddClaimsAsync(user, claims);

                    // Đăng nhập với SignInManager
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "DashBoard", new { area = "Admin" }) });
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
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("DangNhap", "Account", new { area = "Admin" })
                    
            });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi khi đăng xuất: {ex.Message}"
                });
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