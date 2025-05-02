using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using QuanLyNhaHang_DATN.Services.TaiKhoanService;
using QuanLyNhaHang_DATN.ViewModels;
using System;

public class AccountController : Controller
{
    private readonly ITaiKhoanService _taiKhoanService;
    private readonly IKhachHangService _khachHangService;

    public AccountController(ITaiKhoanService taiKhoanService, IKhachHangService khachHangService)
    {
        _taiKhoanService = taiKhoanService;
        _khachHangService = khachHangService;
    }

    [HttpGet]
    public IActionResult DangKy()
    {
        return PartialView("_RegisterModal", new DangKyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangKy(DangKyViewModel model)
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

            if (!model.AgreeTerms)
            {
                return Json(new { success = false, message = "Bạn phải đồng ý với điều khoản dịch vụ để tiếp tục." });
            }

            var result = await _taiKhoanService.DangKyAsync(model);
            if (result.Success)
            {
                return Json(new { success = true, redirectUrl = Url.Action("DangNhap", "Account") });
            }

            var errorMessages = result.Errors.ToList();
            return Json(new { success = false, message = "Đăng ký thất bại", errors = errorMessages });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
        }
    }

    [HttpGet]
    public IActionResult DangNhap()
    {
        return PartialView("_LoginModal", new DangNhapViewModel());
    }

    [HttpPost]
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
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
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
            HttpContext.Session.Clear();
            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Lỗi khi đăng xuất: {ex.Message}" });
        }
    }
}

// thanh toán tt
// admin xử lý đặt bàn  ds chờ -> list bàn trống _> chọn bàn
// hủy bàn, chuyển bàn
// goi món 
// hóa đơn. hóa đơn hủy, chi tiết hóa đơn // xuất hóa đơn
// dashboard
//phân quyền chức năng // policy