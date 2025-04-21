using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Services.TaiKhoanService;
using QuanLyNhaHang_DATN.ViewModels;
using QuanLyNhaHang_DATN.Common;
using System.Threading.Tasks;

namespace QuanLyNhaHang_DATN.Controllers
{
    public class AccountController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;

        public AccountController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_LoginModal", model);
            }

            var result = await _taiKhoanService.DangNhapAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return PartialView("_LoginModal", model);
            }

            // Lưu thông tin vào session
            HttpContext.Session.SetInt32("TaiKhoanId", result.Data.Id);
            HttpContext.Session.SetString("TenTaiKhoan", result.Data.TenTaiKhoan);

            TempData["SuccessMessage"] = "Đăng nhập thành công.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_RegisterModal", model);
            }

            if (!model.AgreeTerms)
            {
                ModelState.AddModelError("AgreeTerms", "Bạn phải đồng ý với điều khoản dịch vụ.");
                return PartialView("_RegisterModal", model);
            }

            var result = await _taiKhoanService.DangKyAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return PartialView("_RegisterModal", model);
            }

            TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công.";
            return RedirectToAction("Index", "Home");
        }
    }
}