using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Hubs;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.BanService;
using QuanLyNhaHang_DATN.Services.DatBanService;
using QuanLyNhaHang_DATN.Services.KhuVucBanService;
using QuanLyNhaHang_DATN.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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
        private readonly IDistributedCache _cache;
        private const double DefaultDurationHours = 2.0;

        public DatBanController(
            IDatBanService datBanService,
            IBanService banService,
            IKhuVucBanService khuVucBanService,
            IHubContext<DatBanHub> hubContext,
            IDistributedCache cache)
        {
            _datBanService = datBanService;
            _banService = banService;
            _khuVucBanService = khuVucBanService;
            _hubContext = hubContext;
            _cache = cache;
        }
        public async Task<IActionResult> DanhSachBanTrong(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null, int? datBanId = null, bool isChuyenBan = false)
        {
            try
            {
                var nhanVienId = User.Claims.FirstOrDefault(c => c.Type == "NhanVienId")?.Value;
                ViewBag.NhanVienId = string.IsNullOrEmpty(nhanVienId) ? 0 : int.Parse(nhanVienId);
                // Thêm ViewBag.IsChuyenBan để view biết đang ở chế độ Chuyển bàn hay Xếp bàn
                ViewBag.IsChuyenBan = isChuyenBan;

                var khuVucBans = await _khuVucBanService.GetAllAsync();
                ViewBag.KhuVucBanList = khuVucBans;
                ViewBag.SearchTenBan = tenBan;
                ViewBag.SelectedKhuVucBanId = khuVucBanId;
                ViewBag.DatBanId = datBanId;

                if (datBanId.HasValue)
                {
                    var datBan = await _datBanService.GetByIdWithKhachHangAsync(datBanId.Value);
                    if (datBan == null)
                    {
                        TempData["Error"] = "Đơn đặt bàn không tồn tại.";
                        return View(new List<Ban>());
                    }

                    ViewBag.DatBanInfo = $"Đơn đặt bàn cho {datBan.KhachHang?.TenKhachHang ?? "Khách vãng lai"} (ID: {datBanId})";
                    ViewBag.RequiredBans = (int)Math.Ceiling((double)datBan.SoLuongNguoi / 6);
                    //
                    (int minAllowedBans, int maxAllowedBans, string tableMessage) = _datBanService.CalculateRequiredTables(datBan.SoLuongNguoi);
                    ViewBag.MinAllowedBans = minAllowedBans;
                    ViewBag.MaxAllowedBans = maxAllowedBans;
                    ViewBag.TableMessage = tableMessage;

                    // Nếu là Chuyển bàn, lấy danh sách bàn hiện tại của đơn để hiển thị
                    if (isChuyenBan)
                    {
                        ViewBag.CurrentBans = await _datBanService.GetBanGhepAsync(datBanId.Value);
                    }
                }

                return View(new List<Ban>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách bàn trống: {ex.Message}";
                return View(new List<Ban>());
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

                var (minAllowedBans, maxAllowedBans, tableMessage) = _datBanService.CalculateRequiredTables(datBan.SoLuongNguoi);
                if (banIds.Length < minAllowedBans || banIds.Length > maxAllowedBans)
                {
                    return Json(new { success = false, message = tableMessage });
                }

                var nhanVienIdClaim = User.FindFirst("NhanVienId")?.Value;
                if (string.IsNullOrEmpty(nhanVienIdClaim) || !int.TryParse(nhanVienIdClaim, out int nhanVienId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên. Vui lòng kiểm tra đăng nhập." });
                }

                var result = await _datBanService.XepBanAsync(datBanId, banIds.ToList(), nhanVienId); // Gọi XepBanAsync
                if (!result.Success)
                {
                    return Json(new { success = false, message = string.Join(", ", result.Errors) });
                }

                var datBanUpdated = result.Data;
                //await _hubContext.Clients.All.SendAsync("ReceiveDatBanUpdate", new
                //{
                //    id = datBanUpdated.Id,
                //    tenKhachHang = datBanUpdated.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                //    sdt = datBanUpdated.KhachHang?.SDT ?? string.Empty,
                //    thoiGianDatBan = datBanUpdated.ThoiGianDatBan,
                //    soLuongNguoi = datBanUpdated.SoLuongNguoi,
                //    cocTien = datBanUpdated.CocTien,
                //    tenLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.TenLienHe : string.Empty,
                //    sdtLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.SDTLienHe : string.Empty,
                //    trangThai = (int)datBanUpdated.TrangThai
                //});
                await NotificationHelper.SendDatBanNotificationAsync(_hubContext, _cache, "XEPBAN", datBanUpdated);


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

        //
        public async Task<IActionResult> GetBanScheduleDetails(int banId, int? datBanId = null, bool isCurrent = false)
        {
            try
            {
                DateTime now = DateTime.Now;
                var query = _datBanService.GetBanSchedules()
                    .Include(bs => bs.DatBan).ThenInclude(db => db.KhachHang)
                    .Include(bs => bs.DatBan).ThenInclude(db => db.NhanVien)
                    .Include(bs => bs.DatBan).ThenInclude(db => db.DatBanBans)
                    .ThenInclude(dbb => dbb.Ban)
                    .Where(bs => bs.BanId == banId);

                if (isCurrent)
                {
                    // Hiển thị lịch đang diễn ra (bao gồm cả lịch bắt đầu trước và kết thúc sau now)
                    query = query.Where(bs => bs.ThoiGianBatDau <= now
                        && (bs.ThoiGianKetThuc == null || bs.ThoiGianKetThuc >= now)
                        && bs.DatBan.TrangThai != TrangThaiBanDat.HoanThanh
                        && bs.DatBan.TrangThai != TrangThaiBanDat.DaHuy);
                }
                else
                {
                    query = query.Where(bs => bs.TrangThai == TrangThaiBan.DaDatTruoc
                        && bs.DatBan.TrangThai == TrangThaiBanDat.DaXacNhan
                        && bs.ThoiGianBatDau > now);
                }

                var banSchedules = await query.OrderBy(bs => bs.ThoiGianBatDau).ToListAsync();

                var schedules = banSchedules.Select(bs => new
                {
                    DatBanId = bs.DatBanId,
                    TenKhachHang = bs.DatBan?.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    SoDienThoai = bs.DatBan?.KhachHang?.SDT ?? string.Empty,
                    ThoiGianDatBan = bs.ThoiGianBatDau.ToString("HH:mm dd/MM/yyyy"),
                    SoLuongKhach = bs.DatBan?.SoLuongNguoi ?? 0,
                    BanGhep = (bs.DatBan != null && bs.DatBan.DatBanBans != null && bs.DatBan.DatBanBans.Any())
                        ? string.Join(", ", bs.DatBan.DatBanBans.Select(dbb => dbb.Ban?.TenBan ?? "Không có tên"))
                        : "Chưa xếp",
                    NhanVienXuLy = bs.DatBan?.NhanVien?.TenNhanVien ?? "Chưa có nhân viên",
                    GhiChu = bs.DatBan?.GhiChu ?? "Không có"
                }).ToList();

                return Json(new { success = true, schedules = schedules });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong GetBanScheduleDetails: {ex.Message}");
                return Json(new { success = false, message = $"Lỗi khi tải lịch sử: {ex.Message}" });
            }
        }

        public async Task<IActionResult> GetPagedBanTrong(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null, int? datBanId = null)
        {
            try
            {
                var filter = new BanFilterModel
                {
                    TenBan = tenBan,
                    KhuVucBanId = khuVucBanId
                };

                var (items, totalCount) = await _banService.GetPagedAsync(pageIndex, pageSize, filter);
                var itemsList = items.ToList();

                int soBanCanThiet = 1;
                var khaDungBan = new Dictionary<int, KhaDungBanViewModel>();

                if (datBanId.HasValue)
                {
                    var datBan = await _datBanService.GetByIdWithKhachHangAsync(datBanId.Value);
                    if (datBan == null)
                    {
                        return Json(new { success = false, message = "Đặt bàn không tồn tại." });
                    }

                    soBanCanThiet = (int)Math.Ceiling((double)datBan.SoLuongNguoi / 6);
                   
                    var thoiGianKetThucDuKien = datBan.ThoiGianDatBan.AddHours(DefaultDurationHours).AddMinutes(30);

                    var currentBanIds = await _datBanService.GetCurrentBanIdsAsync(datBanId.Value);

                    foreach (var ban in itemsList)
                    {
                        var khaDung = await _datBanService.CheckTableAvailabilityAsync(
                            ban.Id,
                            datBan.ThoiGianDatBan,
                            thoiGianKetThucDuKien,
                            datBanId.Value);

                        khaDungBan[ban.Id] = khaDung;
                    }
                }
                else
                {
                    var now = DateTime.Now;
                    var defaultBookingEndTime = now.AddHours(DefaultDurationHours).AddMinutes(30);

                    foreach (var ban in itemsList)
                    {
                        var khaDung = await _datBanService.CheckTableAvailabilityAsync(
                            ban.Id,
                            now,
                            defaultBookingEndTime);

                        khaDungBan[ban.Id] = khaDung;
                    }
                }

                var result = itemsList.Select(item => new
                {
                    Id = item.Id,
                    TenBan = item.TenBan,
                    TenKhuVucBan = item.KhuVucBan?.TenKhuVuc,
                    TrangThaiDisplay = khaDungBan[item.Id].KhaDung ? "Trống" : khaDungBan[item.Id].TrangThai.TrangThaiDisplay,
                    TrangThaiValue = khaDungBan[item.Id].KhaDung ? (int)TrangThaiBan.Trong : khaDungBan[item.Id].TrangThai.TrangThaiValue,
                    ThongTinLichDat = new
                    {
                        KhaDung = khaDungBan[item.Id].KhaDung,
                        LichDat = khaDungBan[item.Id].LichDat.Select(ld => new
                        {
                            ld.DatBanId,
                            ThoiGianBatDau = ld.ThoiGianBatDauFormatted
                        }),
                        IsCurrentTable = khaDungBan[item.Id].IsCurrentTable
                    },
                    IsCurrentTable = datBanId.HasValue && khaDungBan[item.Id].IsCurrentTable
                });

                return Json(new
                {
                    success = true,
                    items = result,
                    totalCount,
                    pageIndex,
                    pageSize,
                    soBanCanThiet
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
        public async Task<IActionResult> ChuyenBan(int datBanId, int[] banIds)
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

                var (minAllowedBans, maxAllowedBans, tableMessage) = _datBanService.CalculateRequiredTables(datBan.SoLuongNguoi);
                if (banIds.Length < minAllowedBans || banIds.Length > maxAllowedBans)
                {
                    return Json(new { success = false, message = tableMessage });
                }

                var nhanVienIdClaim = User.FindFirst("NhanVienId")?.Value;
                if (string.IsNullOrEmpty(nhanVienIdClaim) || !int.TryParse(nhanVienIdClaim, out int nhanVienId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên. Vui lòng kiểm tra đăng nhập." });
                }

                var result = await _datBanService.ChuyenBanAsync(datBanId, banIds.ToList(), nhanVienId); // Gọi ChuyenBanAsync
                if (!result.Success)
                {
                    return Json(new { success = false, message = string.Join(", ", result.Errors) });
                }

                var datBanUpdated = result.Data;
                //await _hubContext.Clients.All.SendAsync("ReceiveDatBanUpdate", new
                //{
                //    id = datBanUpdated.Id,
                //    tenKhachHang = datBanUpdated.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                //    sdt = datBanUpdated.KhachHang?.SDT ?? string.Empty,
                //    thoiGianDatBan = datBanUpdated.ThoiGianDatBan,
                //    soLuongNguoi = datBanUpdated.SoLuongNguoi,
                //    cocTien = datBanUpdated.CocTien,
                //    tenLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.TenLienHe : string.Empty,
                //    sdtLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.SDTLienHe : string.Empty,
                //    trangThai = (int)datBanUpdated.TrangThai
                //});
                //await _hubContext.Clients.Group("Admins").SendAsync("SendBookingNotification", datBanUpdated, "UPDATE");


               await NotificationHelper.SendDatBanNotificationAsync(_hubContext, _cache, "CHUYENBAN", datBanUpdated);


                return Json(new { success = true, message = "Chuyển bàn thành công!" });
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[ERROR] ChuyenBan failed: {ex.Message} at {DateTime.Now}");
                return Json(new { success = false, message = $"Lỗi khi chuyển bàn: {ex.Message}" });
            }
        }

        // Action mới: Xử lý hủy bàn, cập nhật trạng thái và xóa liên kết
        [HttpGet]
        public async Task<IActionResult> HuyBanForm(int datBanId)
        {
            var datBan = await _datBanService.GetByIdWithDetailsAsync(datBanId);
            if (datBan == null)
            {
                return Json(new { success = false, message = "Đơn đặt bàn không tồn tại." });
            }

            var viewModel = new DatBanViewModel
            {
                Id = datBan.Id,
                TenKhachHang = datBan.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                SDT = datBan.KhachHang?.SDT ?? "",
                ThoiGianDatBan = datBan.ThoiGianDatBan,
                SoLuongNguoi = datBan.SoLuongNguoi,
                Bans = datBan.DatBanBans != null && datBan.DatBanBans.Any()
                ? string.Join(", ", datBan.DatBanBans.Select(dbb => dbb.Ban.TenBan)): "Không có bàn",
                TenLienHe = datBan.TenLienHe ?? "",
                SDTLienHe = datBan.SDTLienHe ?? "",
                TenNhanVien = datBan.NhanVien?.TenNhanVien ?? "",
                TrangThaiDisplay = GetEnumDisplayName(datBan.TrangThai),
                LyDoHuy = ""
            };

            return PartialView("_HuyDatBanPartial", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyBan(int datBanId, string lyDoHuy)
        {
            try
            {
                var nhanVienIdClaim = User.FindFirst("NhanVienId")?.Value;
                if (string.IsNullOrEmpty(nhanVienIdClaim) || !int.TryParse(nhanVienIdClaim, out int nhanVienId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên. Vui lòng kiểm tra đăng nhập." });
                }

                var result = await _datBanService.HuyBanAsync(datBanId, nhanVienId, lyDoHuy);
                if (!result.Success)
                {
                    return Json(new { success = false, message = string.Join(", ", result.Errors) });
                }

                var datBanUpdated = result.Data;
                //await _hubContext.Clients.All.SendAsync("ReceiveDatBanUpdate", new
                //{
                //    id = datBanUpdated.Id,
                //    tenKhachHang = datBanUpdated.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                //    sdt = datBanUpdated.KhachHang?.SDT ?? string.Empty,
                //    thoiGianDatBan = datBanUpdated.ThoiGianDatBan,
                //    soLuongNguoi = datBanUpdated.SoLuongNguoi,
                //    cocTien = datBanUpdated.CocTien,
                //    tenLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.TenLienHe : string.Empty,
                //    sdtLienHe = datBanUpdated.IsDatHo == true ? datBanUpdated.SDTLienHe : string.Empty,
                //    trangThai = (int)datBanUpdated.TrangThai

                //});
                await NotificationHelper.SendDatBanNotificationAsync(_hubContext, _cache, "HUY", datBanUpdated);


                return Json(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi hủy bàn: {ex.Message}" });
            }
        }

        public async Task<IActionResult> DanhSachBanHienThoi(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null)
        {
            try
            {
                var khuVucBans = await _khuVucBanService.GetAllAsync();
                ViewBag.KhuVucBanList = khuVucBans;
                ViewBag.SearchTenBan = tenBan;
                ViewBag.SelectedKhuVucBanId = khuVucBanId;

                var filter = new BanFilterModel
                {
                    TenBan = tenBan,
                    KhuVucBanId = khuVucBanId
                };
                var (items, totalCount) = await _banService.GetPagedAsync(pageIndex, pageSize, filter);
                ViewBag.BanList = items.ToList();
                ViewBag.TotalCount = totalCount;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách bàn hiện thời: {ex.Message}";
                return View(new List<Ban>());
            }
        }
        [HttpGet]
       
        public async Task<IActionResult> GetPagedBanHienThoi(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null)
        {
            try
            {
                var filter = new BanFilterModel
                {
                    TenBan = tenBan,
                    KhuVucBanId = khuVucBanId
                };

                var (items, totalCount) = await _banService.GetPagedAsync(pageIndex, pageSize, filter);
                var itemsList = items.ToList();

                var khaDungBan = new Dictionary<int, KhaDungBanViewModel>();
                var currentTime = DateTime.Now;

                foreach (var ban in itemsList)
                {
                    var trangThai = await _datBanService.GetTableStatusAsync(ban.Id, currentTime);
                    var khaDung = await _datBanService.CheckTableAvailabilityAsync(ban.Id, currentTime, currentTime.AddHours(DefaultDurationHours).AddMinutes(30));
                    khaDungBan[ban.Id] = khaDung;
                }

                var result = itemsList.Select(item => new
                {
                    Id = item.Id,
                    TenBan = item.TenBan,
                    TenKhuVucBan = item.KhuVucBan?.TenKhuVuc,
                    TrangThaiDisplay = khaDungBan[item.Id].TrangThai.TrangThaiDisplay,
                    TrangThaiValue = khaDungBan[item.Id].TrangThai.TrangThaiValue,
                    ThongTinLichDat = new
                    {
                        KhaDung = khaDungBan[item.Id].KhaDung,
                        LichDat = khaDungBan[item.Id].LichDat.Select(ld => new
                        {
                            ld.DatBanId,
                            ThoiGianBatDau = ld.ThoiGianBatDauFormatted
                        })
                    }
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
        // create
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreateDatBanPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDatBan(DatBanViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                var username = User.Identity?.Name;

                // Gán giá trị mặc định
                viewModel.CocTien = 0;
                viewModel.Loai = LoaiDatBan.DatKhiDen;
                viewModel.IsDatHo = false;

                var result = await _datBanService.CreateDatBanAsync(viewModel, username);

                if (!result.Success)
                    return Json(new { success = false, message = string.Join(", ", result.Errors) });

                var datBan = result.Data;

                //await _hubContext.Clients.All.SendAsync("ReceiveDatBanUpdate", new
                //{
                //    id = datBan.Id,
                //    tenKhachHang = datBan.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                //    sdt = datBan.KhachHang?.SDT ?? string.Empty,
                //    thoiGianDatBan = datBan.ThoiGianDatBan,
                //    soLuongNguoi = datBan.SoLuongNguoi,
                //    cocTien = datBan.CocTien,
                //    tenLienHe = datBan.IsDatHo == true ? datBan.TenLienHe : string.Empty,
                //    sdtLienHe = datBan.IsDatHo == true ? datBan.SDTLienHe : string.Empty,
                //    trangThai = (int)datBan.TrangThai
                //});
                // Cuối phương thức
                await NotificationHelper.SendDatBanNotificationAsync(_hubContext, _cache, "NEW", datBan);

                return Json(new { success = true, message = "Tạo đơn đặt bàn thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi tạo đơn đặt bàn: {ex.Message}" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var datBan = await _datBanService.GetDatBanByIdAsync(id);
            if (datBan == null)
            {
                return NotFound();
            }

            var viewModel = new DatBanViewModel
            {
                Id = datBan.Id,
                TenKhachHang = datBan.KhachHang?.TenKhachHang ?? "",
                SDT = datBan.KhachHang?.SDT ?? "",
                ThoiGianDatBan = datBan.ThoiGianDatBan,
                SoLuongNguoi = datBan.SoLuongNguoi,
                GhiChu = datBan.GhiChu
            };

            return PartialView("_EditDatBanPartial", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DatBanViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                var username = User.Identity?.Name;

                // Gán giá trị mặc định 
                viewModel.CocTien = 0; 
                viewModel.Loai = LoaiDatBan.DatKhiDen; 
                viewModel.IsDatHo = false; 

                var result = await _datBanService.UpdateDatBanAsync(viewModel, username);

                if (!result.Success)
                    return Json(new { success = false, message = string.Join(", ", result.Errors) });

                var datBan = result.Data;

                //await _hubContext.Clients.Group("Admins").SendAsync("ReceiveBookingNotification", datBan, "UPDATE");

                return Json(new { success = true, message = "Cập nhật đơn đặt bàn thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi cập nhật đơn đặt bàn: {ex.Message}" });
            }
        }
        public async Task<IActionResult> LichSuDatBan(int pageIndex = 1, int pageSize = 5, string tenKhachHang = null, string sdt = null, int? trangThai = null)
        {
            try
            {
                var filter = new DatBanFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    TrangThai = trangThai.HasValue ? (TrangThaiBanDat)trangThai : null // Cho phép lọc HoanThanh hoặc DaHuy
                };

                var (items, totalCount) = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

                var viewModels = items.Select(db => new DatBanViewModel
                {
                    Id = db.Id,
                    TenKhachHang = db.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    SDT = db.KhachHang?.SDT ?? "",
                    ThoiGianDatBan = db.ThoiGianDatBan,
                    SoLuongNguoi = db.SoLuongNguoi,
                    CocTien = db.CocTien,
                    Bans = db.DatBanBans != null ? string.Join(", ", db.DatBanBans.Select(dbb => dbb.Ban.TenBan)) : "Không có bàn",
                    IsDatHo = db.IsDatHo ?? false,
                    TenLienHe = db.IsDatHo == true ? db.TenLienHe : "",
                    SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : "",
                    TenNhanVien = db.NhanVien?.TenNhanVien ?? "",
                    TrangThai = db.TrangThai,
                    TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                    LyDoHuy = db.LyDoHuy
                }).ToList();

                ViewBag.SearchTenKhachHang = tenKhachHang;
                ViewBag.SearchSDT = sdt;
                ViewBag.PageIndex = pageIndex;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.TrangThai = trangThai;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải lịch sử đặt bàn: {ex.Message}";
                return View(new List<DatBanViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedLichSuDatBan(int pageIndex = 1, int pageSize = 5, string tenKhachHang = null, string sdt = null, int trangThai = 0)
        {
            try
            {
                var filter = new DatBanFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    TrangThai = (TrangThaiBanDat)trangThai // 3: HoanThanh, 2: DaHuy
                };

                var (items, totalCount) = await _datBanService.GetPagedAsync(pageIndex, pageSize, filter);

                var pagedItems = items.Select(db => new
                {
                    Id = db.Id,
                    TenKhachHang = db.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    SDT = db.KhachHang?.SDT ?? "",
                    ThoiGianDatBan = db.ThoiGianDatBan,
                    SoLuongNguoi = db.SoLuongNguoi,
                    CocTien = db.CocTien,
                    DanhSachBanGhep = db.DatBanBans != null ? string.Join(", ", db.DatBanBans.Select(dbb => dbb.Ban.TenBan)) : "Không có bàn",
                    TenLienHe = db.IsDatHo == true ? db.TenLienHe : "",
                    SDTLienHe = db.IsDatHo == true ? db.SDTLienHe : "",
                    TenNhanVien = db.NhanVien?.TenNhanVien ?? "",
                    TrangThaiDisplay = GetEnumDisplayName(db.TrangThai),
                    TrangThai = (int)db.TrangThai,
                    LyDoHuy = db.LyDoHuy ?? "Không có"
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
        public async Task<IActionResult> GetUserNotifications()
        {
            var key = "notifs:all";
            var notificationsJson = await _cache.GetStringAsync(key) ?? "[]";
            var notifications = JsonSerializer.Deserialize<List<NotificationModel>>(notificationsJson) ?? new List<NotificationModel>();
            return Json(new { success = true, data = notifications });
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
