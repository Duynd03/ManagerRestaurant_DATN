using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.NhanVienService;
using QuanLyNhaHang_DATN.Services.TaiKhoanService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using QuanLyNhaHang_DATN.ViewModels;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;

namespace QuanLyNhaHang_DATN.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NhanVienController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly INhanVienService _nhanVienService;
        private readonly UserManager<TaiKhoan> _userManager;

        public NhanVienController(ITaiKhoanService taiKhoanService, INhanVienService nhanVienService, UserManager<TaiKhoan> userManager)
        {
            _taiKhoanService = taiKhoanService;
            _nhanVienService = nhanVienService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 5, string tenNhanVien = null, int? quyenId = null)
        {
            var filter = new NhanVienFilterModel
            {
                TenNhanVien = tenNhanVien,
                QuyenId = quyenId
            };
            var (items, totalCount) = await _nhanVienService.GetPagedAsync(pageIndex, pageSize, filter);
            var quyenList = await _taiKhoanService.GetAllQuyenAsync();
            ViewBag.SearchTenNhanVien = tenNhanVien;
            ViewBag.SelectedQuyenId = quyenId;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.QuyenList = quyenList;

            return View(items);
        }

        public async Task<IActionResult> GetPagedData(int pageIndex = 1, int pageSize = 5, string tenNhanVien = null, int? quyenId = null)
        {
            var filter = new NhanVienFilterModel
            {
                TenNhanVien = tenNhanVien,
                QuyenId = quyenId
            };

            var (items, totalCount) = await _nhanVienService.GetPagedAsync(pageIndex, pageSize, filter);

            if (items == null || !items.Any())
            {
                return Json(new
                {
                    items = new List<object>(),
                    totalCount = 0,
                    pageIndex,
                    pageSize,
                    search = tenNhanVien
                });
            }

            var data = items.Select(item => new
            {
                id = item.Id,
                tenNhanVien = item.TenNhanVien ?? "Chưa có",
                sdt = item.Sdt ?? "Chưa có",
                ngaySinh = item.NgaySinh?.ToString("dd/MM/yyyy") ?? "",
                diaChi = item.DiaChi ?? "",
                username = item.TaiKhoan?.UserName ?? "N/A",
                quyen = item.TaiKhoan?.Quyen?.Name ?? "N/A",
                trangThai = item.TaiKhoan?.TrangThai.ToString() ?? "N/A",
                trangThaiDisplay = item.TaiKhoan != null ? GetEnumDisplayName(item.TaiKhoan.TrangThai) : "N/A",
                taiKhoanId = item.TaiKhoanId
            });

            return Json(new
            {
                items = data,
                totalCount,
                pageIndex,
                pageSize,
                search = tenNhanVien
            });
        }

        public async Task<IActionResult> Create()
        {
            var quyenList = await _taiKhoanService.GetAllQuyenAsync();
            ViewBag.QuyenList = quyenList;
            return PartialView("_CreatePartial", new CreateNhanVienViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateNhanVienViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = validationErrors });
                }

                var result = await _taiKhoanService.CreateUserAsync(
                    model.Username,
                    model.Password,
                    model.QuyenId,
                    model.TenNhanVien,
                    model.Sdt,
                    model.NgaySinh,
                    model.DiaChi);

                if (result.Success)
                {
                    return Json(new { success = true, message = "Thêm nhân viên thành công" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var nhanVien = await _nhanVienService.GetByIdAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            var model = new UpdateNhanVienViewModel
            {
                Id = nhanVien.Id,
                TenNhanVien = nhanVien.TenNhanVien,
                Sdt = nhanVien.Sdt,
                NgaySinh = nhanVien.NgaySinh,
                DiaChi = nhanVien.DiaChi,
                QuyenId = nhanVien.TaiKhoan?.QuyenId ?? 0
            };

            ViewBag.QuyenList = await _taiKhoanService.GetAllQuyenAsync();
            return PartialView("_EditPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UpdateNhanVienViewModel model)
        {
            try
            {
                if (model.Id <= 0)
                {
                    return Json(new { success = false, message = "Id nhân viên không hợp lệ." });
                }
                Console.WriteLine($"Id từ form: {model.Id}");
                if (!ModelState.IsValid)
                {
                    var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = validationErrors });
                }

                var result = await _taiKhoanService.UpdateUserAsync(
                    model.Id,
                    model.TenNhanVien,
                    model.Sdt,
                    model.NgaySinh,
                    model.DiaChi,
                    model.QuyenId);

                if (result.Success)
                {
                    return Json(new { success = true, message = "Cập nhật nhân viên thành công" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ChangePassword(int taiKhoanId)
        {
            var taiKhoan = await _userManager.FindByIdAsync(taiKhoanId.ToString());
            if (taiKhoan == null)
            {
                return NotFound();
            }

            var model = new ChangePasswordViewModel
            {
                TaiKhoanId = taiKhoanId
            };
            return PartialView("_ChangePasswordPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = validationErrors });
                }

                var taiKhoan = await _userManager.FindByIdAsync(model.TaiKhoanId.ToString());
                if (taiKhoan == null)
                {
                    return Json(new { success = false, message = "Tài khoản không tồn tại" });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(taiKhoan);
                var result = await _userManager.ResetPasswordAsync(taiKhoan, token, model.NewPassword);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Đổi mật khẩu thành công" });
                }

                var identityErrors = string.Join("\n", result.Errors.Select(e => e.Description));
                return Json(new { success = false, message = $"Đổi mật khẩu thất bại:\n{identityErrors}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int taiKhoanId)
        {
            var taiKhoan = await _userManager.FindByIdAsync(taiKhoanId.ToString());
            if (taiKhoan == null)
            {
                return Json(new { success = false, message = "Tài khoản không tồn tại." });
            }

            taiKhoan.TrangThai = TrangThaiTaiKhoan.DaKhoa; // Chuyển sang Đã khóa
            var result = await _userManager.UpdateAsync(taiKhoan);

            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Khóa tài khoản thất bại." });
            }

            return Json(new { success = true, message = "Khóa tài khoản thành công." });
        }
        private string GetEnumDisplayName<TEnum>(TEnum value) where TEnum : Enum
        {
            var memberInfo = value.GetType().GetMember(value.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DisplayAttribute)attrs[0]).Name;
            }
            return value.ToString();
        }
    }

}