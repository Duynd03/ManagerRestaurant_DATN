﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Hubs;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.BanService;
using QuanLyNhaHang_DATN.Services.DatBanService;
using QuanLyNhaHang_DATN.Services.KhuVucBanService;
using QuanLyNhaHang_DATN.ViewModels;
using System.ComponentModel.DataAnnotations; 
namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class DatBanController : Controller
    {
        private readonly IDatBanService _datBanService;
        private readonly IBanService _banService;
        private readonly IKhuVucBanService _khuVucBanService;
        private readonly IHubContext<DatBanHub> _hubContext;

        public DatBanController(
            IDatBanService datBanService,
            IBanService banService,
            IKhuVucBanService khuVucBanService,
            IHubContext<DatBanHub> hubContext)
        {
            _datBanService = datBanService;
            _banService = banService;
            _khuVucBanService = khuVucBanService;
            _hubContext = hubContext;
        }
        //public async Task<IActionResult> DanhSachBanTrong(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null, int? datBanId = null)
        //{
        //    try
        //    {
        //        var filter = new BanFilterModel
        //        {
        //            TenBan = tenBan,
        //            KhuVucBanId = khuVucBanId,
        //            TrangThai = (int)TrangThaiBan.Trong
        //        };

        //        var (items, totalCount) = await _banService.GetPagedAsync(pageIndex, pageSize, filter);
        //        var khuVucBans = await _khuVucBanService.GetAllAsync();

        //        ViewBag.SearchTenBan = tenBan;
        //        ViewBag.SelectedKhuVucBanId = khuVucBanId;
        //        ViewBag.PageIndex = pageIndex;
        //        ViewBag.PageSize = pageSize;
        //        ViewBag.TotalCount = totalCount;
        //        ViewBag.KhuVucBanList = khuVucBans;
        //        ViewBag.DatBanId = datBanId;

        //        if (datBanId.HasValue)
        //        {
        //            var datBan = await _datBanService.GetByIdWithKhachHangAsync(datBanId.Value); // Sửa từ GetByIdAsync
        //            if (datBan != null)
        //            {
        //                ViewBag.DatBanInfo = $"Đơn đặt bàn cho {datBan.KhachHang?.TenKhachHang ?? "Khách vãng lai"} (ID: {datBanId})";
        //                ViewBag.RequiredBans = (int)Math.Ceiling((double)datBan.SoLuongNguoi / 6);
        //            }
        //        }

        //        return View(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = $"Lỗi khi tải danh sách bàn trống: {ex.Message}";
        //        return View(new List<Ban>());
        //    }
        //}

        public async Task<IActionResult> DanhSachBanTrong(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null, int? datBanId = null)
        {
            try
            {
                // Lấy IdNhanVien từ Claims
                var nhanVienId = User.Claims.FirstOrDefault(c => c.Type == "NhanVienId")?.Value;
                ViewBag.NhanVienId = string.IsNullOrEmpty(nhanVienId) ? 0 : int.Parse(nhanVienId);

                var khuVucBans = await _khuVucBanService.GetAllAsync();
                ViewBag.KhuVucBanList = khuVucBans;
                ViewBag.SearchTenBan = tenBan;
                ViewBag.SelectedKhuVucBanId = khuVucBanId;
                ViewBag.DatBanId = datBanId;

                if (datBanId.HasValue)
                {
                    var datBan = await _datBanService.GetByIdWithKhachHangAsync(datBanId.Value);
                    if (datBan != null)
                    {
                        ViewBag.DatBanInfo = $"Đơn đặt bàn cho {datBan.KhachHang?.TenKhachHang ?? "Khách vãng lai"} (ID: {datBanId})";
                        ViewBag.RequiredBans = (int)Math.Ceiling((double)datBan.SoLuongNguoi / 6);
                    }
                }

                // Trả về view với danh sách rỗng, dữ liệu sẽ được tải qua AJAX
                return View(new List<Ban>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách bàn trống: {ex.Message}";
                return View(new List<Ban>());
            }
        }
        public async Task<IActionResult> GetPagedBanTrong(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null, int? datBanId = null)
        {
            try
            {
                var filter = new BanFilterModel
                {
                    TenBan = tenBan,
                    KhuVucBanId = khuVucBanId,
                    TrangThai = (int)TrangThaiBan.Trong
                };

                var (items, totalCount) = await _banService.GetPagedAsync(pageIndex, pageSize, filter);
                int requiredBans = 1;
                if (datBanId.HasValue)
                {
                    var datBan = await _datBanService.GetByIdWithKhachHangAsync(datBanId.Value); // Sửa từ GetByIdAsync
                    if (datBan != null)
                    {
                        requiredBans = (int)Math.Ceiling((double)datBan.SoLuongNguoi / 6);
                    }
                }

                var result = items.Select(item => new
                {
                    Id = item.Id,
                    TenBan = item.TenBan,
                    TenKhuVucBan = item.KhuVucBan?.TenKhuVuc,
                    TrangThaiDisplay = GetEnumDisplayName(item.TrangThai),
                    TrangThaiValue = (int)item.TrangThai
                });

                return Json(new
                {
                    success = true,
                    items = result,
                    totalCount,
                    pageIndex,
                    pageSize,
                    requiredBans
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi khi tải dữ liệu: {ex.Message}"
                });
            }
        }
      
        public async Task<IActionResult> DanhSachKhachHangChoXepBan(int pageIndex = 1, int pageSize = 5, string tenKhachHang = null, string sdt = null)
        {
            try
            {
                var filter = new DatBanFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    TrangThai = TrangThaiBanDat.ChoXacNhan
                };

                var (items, totalCount) = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

                var viewModels = items.Select(db => new DatBanViewModel
                {
                    Id = db.Id,
                    TenKhachHang = db.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    SDT = db.KhachHang?.SDT ?? string.Empty,
                    ThoiGianDatBan = db.ThoiGianDatBan,
                    SoLuongNguoi = db.SoLuongNguoi,
                    CocTien = db.CocTien,
                    IsDatHo = db.IsDatHo ?? false,
                    TenLienHe = db.IsDatHo == true ? db.TenLienHe : string.Empty,
                    SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : string.Empty,
                    TrangThai = db.TrangThai,
                    TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                    Loai = db.Loai,
                    GhiChu = db.GhiChu,
                    Bans = db.DatBanBans.Any()
                        ? string.Join(", ", db.DatBanBans.Select(dbb => dbb.Ban.TenBan))
                        : (db.SoLuongNguoi > 6 ? "Chưa ghép bàn" : "Chưa xếp bàn")
                }).ToList();

                ViewBag.SearchTenKhachHang = tenKhachHang;
                ViewBag.SearchSDT = sdt;
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách khách hàng chờ xếp bàn: {ex.Message}";
                return View(new List<DatBanViewModel>());
            }
        }
        public async Task<IActionResult> GetPagedKhachHangChoXepBan(int pageIndex = 1, int pageSize = 5, string tenKhachHang = null, string sdt = null)
        {
            try
            {
                var filter = new DatBanFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    TrangThai = TrangThaiBanDat.ChoXacNhan
                };

                var (items, totalCount) = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

                var pagedItems = items.Select(db => new
                {
                    Id = db.Id,
                    TenKhachHang = db.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    SDT = db.KhachHang?.SDT ?? string.Empty,
                    ThoiGianDatBan = db.ThoiGianDatBan,
                    SoLuongNguoi = db.SoLuongNguoi,
                    CocTien = db.CocTien,
                    TenLienHe = db.IsDatHo == true ? db.TenLienHe : string.Empty,
                    SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : string.Empty,
                    TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                    TrangThai = (int)db.TrangThai,
                    DanhSachBanGhep = db.DatBanBans.Any()
                        ? string.Join(", ", db.DatBanBans.Select(dbb => dbb.Ban.TenBan))
                        : (db.SoLuongNguoi > 6 ? "Chưa ghép bàn" : "Chưa xếp bàn")
                }).ToList();

                return Json(new
                {
                    success = true,
                    items = pagedItems,
                    totalCount,
                    pageIndex,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi khi tải dữ liệu: {ex.Message}"
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XepBan(int datBanId, int[] banIds)
        {

            try
            {
                if (banIds == null || banIds.Length == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn ít nhất một bàn." });
                }

                var datBan = await _datBanService.GetByIdWithKhachHangAsync(datBanId);
                if (datBan == null)
                {
                    return Json(new { success = false, message = "Đặt bàn không tồn tại." });
                }

                int requiredBans = (int)Math.Ceiling((double)datBan.SoLuongNguoi / 6);
                if (banIds.Length < requiredBans)
                {
                    return Json(new { success = false, message = $"Cần chọn ít nhất {requiredBans} bàn cho {datBan.SoLuongNguoi} người." });
                }
                // Lấy NhanVienId từ User.Claims
                var nhanVienIdClaim = User.FindFirst("NhanVienId")?.Value;
                if (string.IsNullOrEmpty(nhanVienIdClaim) || !int.TryParse(nhanVienIdClaim, out int nhanVienId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên. Vui lòng kiểm tra đăng nhập." });
                }
                var result = await _datBanService.XepBanAsync(datBanId, banIds.ToList(), nhanVienId);
                if (!result.Success)
                {
                    return Json(new { success = false, message = string.Join(", ", result.Errors) });
                }

                var datBanUpdated = result.Data;
                await _hubContext.Clients.All.SendAsync("ReceiveDatBanUpdate", new
                {
                    id = datBanUpdated.Id,
                    tenKhachHang = datBanUpdated.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    sdt = datBanUpdated.KhachHang?.SDT ?? string.Empty,
                    thoiGianDatBan = datBanUpdated.ThoiGianDatBan,
                    soLuongNguoi = datBanUpdated.SoLuongNguoi,
                    cocTien = datBanUpdated.CocTien,
                    tenLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.TenLienHe : string.Empty,
                    sdtLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.SDTLienHe : string.Empty,
                    trangThai = (int)datBanUpdated.TrangThai
                });

                return Json(new { success = true, message = "Xếp bàn thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xếp bàn: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBanGhep(int datBanId)
        {
            var banNames = await _datBanService.GetBanGhepAsync(datBanId);
            return Json(new { bans = string.Join(", ", banNames) });
        }
        public async Task<IActionResult> LichDatBan(int pageIndex = 1, int pageSize = 5, string tenKhachHang = null, string sdt = null)
        {
            try
            {
                var filter = new DatBanFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    TrangThai = TrangThaiBanDat.DaXacNhan
                };

                var (items, totalCount) = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

                var viewModels = items.Select(db => new DatBanViewModel
                {
                    Id = db.Id,
                    TenKhachHang = db.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    SDT = db.KhachHang?.SDT ?? string.Empty,
                    ThoiGianDatBan = db.ThoiGianDatBan,
                    SoLuongNguoi = db.SoLuongNguoi,
                    CocTien = db.CocTien,
                    IsDatHo = db.IsDatHo ?? false,
                    TenLienHe = db.IsDatHo == true ? db.TenLienHe : string.Empty,
                    SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : string.Empty,
                    TrangThai = db.TrangThai,
                    TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                    Bans = db.DatBanBans.Any()
                        ? string.Join(", ", db.DatBanBans.Select(dbb => dbb.Ban.TenBan))
                        : "Không có bàn"
                }).ToList();

                ViewBag.SearchTenKhachHang = tenKhachHang;
                ViewBag.SearchSDT = sdt;
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải lịch sử xếp bàn: {ex.Message}";
                return View(new List<DatBanViewModel>());
            }
        }

        public async Task<IActionResult> GetPagedLichDatBan(int pageIndex = 1, int pageSize = 5, string tenKhachHang = null, string sdt = null)
        {
            try
            {
                var filter = new DatBanFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    TrangThai = TrangThaiBanDat.DaXacNhan
                };

                var (items, totalCount) = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

                var pagedItems = items.Select(db => new
                {
                    Id = db.Id,
                    TenKhachHang = db.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    SDT = db.KhachHang?.SDT ?? string.Empty,
                    ThoiGianDatBan = db.ThoiGianDatBan,
                    SoLuongNguoi = db.SoLuongNguoi,
                    CocTien = db.CocTien,
                    TenLienHe = db.IsDatHo == true ? db.TenLienHe : string.Empty,
                    SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : string.Empty,
                    TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                    TrangThai = (int)db.TrangThai,
                    TenNhanVien = db.NhanVien?.TenNhanVien ?? " ",
                    DanhSachBanGhep = db.DatBanBans.Any()
                        ? string.Join(", ", db.DatBanBans.Select(dbb => dbb.Ban.TenBan))
                        : "Không có bàn"
                }).ToList();

                return Json(new
                {
                    success = true,
                    items = pagedItems,
                    totalCount,
                    pageIndex,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi khi tải dữ liệu: {ex.Message}"
                });
            }
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