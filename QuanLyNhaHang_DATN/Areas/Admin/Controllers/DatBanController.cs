using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
   // [Authorize(Roles = "Admin,NhanVien")]
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

        public async Task<IActionResult> DanhSachBanTrong(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null)
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
                var khuVucBans = await _khuVucBanService.GetAllAsync();

                ViewBag.SearchTenBan = tenBan;
                ViewBag.SelectedKhuVucBanId = khuVucBanId;
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.KhuVucBanList = khuVucBans;

                return View(items);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách bàn trống: {ex.Message}";
                return View(new List<Ban>());
            }
        }

        public async Task<IActionResult> GetPagedBanTrong(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null)
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
        public async Task<IActionResult> DanhSachKhachHangChoXepBan(int pageIndex = 1, int pageSize = 5, string tenKhachHang = null, string sdt = null)
        {
            try
            {
                var filter = new DatBanFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    TrangThai = null 
                };

                var result = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

             
                var items = result.Items
                    .Where(db => (db.TrangThai == TrangThaiBanDat.ChoXacNhan ) && db.BanId == null)
                    .ToList();

                var totalCount = items.Count;

                var viewModels = items
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(db => new DatBanViewModel
                    {
                        Id = db.Id,
                        TenKhachHang = db.KhachHang.TenKhachHang,
                        SDT = db.KhachHang.SDT,
                        //TenKhachHang = db.KhachHang != null ? db.KhachHang.TenKhachHang : "Khách vãng lai", // Kiểm tra null
                        //SDT = db.KhachHang != null ? db.KhachHang.SDT : string.Empty,
                        ThoiGianDatBan = db.ThoiGianDatBan,
                        SoLuongNguoi = db.SoLuongNguoi,
                        CocTien = db.CocTien,
                        IsDatHo = db.IsDatHo ?? false,
                        TenLienHe = db.IsDatHo == true ? db.TenLienHe : string.Empty,
                        SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : string.Empty,
                        TrangThai = db.TrangThai,
                        TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                        BanId = db.BanId,
                        Loai = db.Loai,
                        GhiChu = db.GhiChu
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
                    TrangThai = null
                };

                var result = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

                var items = result.Items
                    .Where(db => (db.TrangThai == TrangThaiBanDat.ChoXacNhan) && db.BanId == null)
                    .ToList();

                var totalCount = items.Count;

                var pagedItems = items
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(db => new
                    {
                        Id = db.Id,
                        TenKhachHang = db.KhachHang != null ? db.KhachHang.TenKhachHang : "Khách vãng lai",
                        SDT = db.KhachHang != null ? db.KhachHang.SDT : string.Empty,
                        ThoiGianDatBan = db.ThoiGianDatBan,
                        SoLuongNguoi = db.SoLuongNguoi,
                        CocTien = db.CocTien,
                        TenLienHe = db.IsDatHo == true ? db.TenLienHe : string.Empty,
                        SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : string.Empty,
                        TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                        TrangThai = (int)db.TrangThai,
                        BanId = db.BanId
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