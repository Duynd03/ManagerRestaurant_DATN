using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services;
using QuanLyNhaHang_DATN.Services.GoiMonService;
using QuanLyNhaHang_DATN.Services.HoaDonService;
using QuanLyNhaHang_DATN.Services.MonAnService;
using QuanLyNhaHang_DATN.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,KeToan")]
    public class HoaDonController : Controller
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IGoiMonService _goiMonService;
        private readonly IMonAnService _monAnService;
        private readonly IConverter _converter; // DinkToPdf converter
        private readonly ICompositeViewEngine _viewEngine; // Để render view thành string
        private const decimal VatRate = 0.10m; // Tỷ lệ VAT 10%
        public HoaDonController(
            IHoaDonService hoaDonService,
            IGoiMonService goiMonService,
            IMonAnService monAnService,
            IConverter converter,
            ICompositeViewEngine viewEngine)
        {
            _hoaDonService = hoaDonService;
            _goiMonService = goiMonService;
            _monAnService = monAnService;
            _converter = converter;
            _viewEngine = viewEngine;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHoaDon(int datBanId)
        {
            if (datBanId <= 0)
            {
                return Json(new { success = false, message = "Đặt bàn không hợp lệ!" });
            }

            try
            {
                var existingHoaDon = await _hoaDonService.GetHoaDonChuaThanhToanByDatBanIdAsync(datBanId);
                if (existingHoaDon != null)
                {
                    return Json(new { success = true, message = "Hóa đơn đã được tạo trước đó, vui lòng liên hệ Kế toán để xử lý!" });
                }
                // Kiểm tra xem có món ăn được gọi hay không
                var goiMons = await _goiMonService.GetByDatBanIdAsync(datBanId);
                if (goiMons == null || !goiMons.Any())
                {
                    return Json(new { success = false, message = "Chưa có món ăn nào được lưu! Vui lòng chọn và lưu danh sách món trước khi tạo hóa đơn." });
                }
                await _hoaDonService.CreateHoaDonAsync(datBanId);
                return Json(new { success = true, message = "Tạo hóa đơn thành công! Vui lòng liên hệ Kế toán để xử lý." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi tạo hóa đơn: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ChiTietHoaDon(int id)
        {
            var hoaDon = await _hoaDonService.GetByIdWithDetailsAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            var goiMons = await _goiMonService.GetByDatBanIdAsync(hoaDon.DatBanId);
            var monAns = await _monAnService.GetAllAsync();
           

            var goiMonViewModels = goiMons
            .GroupBy(gm => gm.MonAnId) // Gộp theo MonAnId
            .Select(g => new GoiMonViewModel
            {
                MonAnId = g.Key,
                SoLuong = g.Sum(x => x.SoLuong),
                Gia = g.First().Gia,
                TenMonAn = monAns.FirstOrDefault(m => m.Id == g.Key)?.TenMonAn ?? "Món không xác định"
            }).ToList();

            var banNames = hoaDon.DatBan?.DatBanBans?.Any() == true
                ? string.Join(", ", hoaDon.DatBan.DatBanBans.Select(dbb => dbb.Ban.TenBan))
                : "Chưa ghép bàn";

            // Tính thuế VAT động để hiển thị
            var tienCoc = hoaDon.DatBan?.CocTien ?? 0;
            var soTienTruocVat = hoaDon.TongTienGoiMon - tienCoc;
            var thueVat = soTienTruocVat > 0 ? soTienTruocVat * VatRate : 0;

            var hoaDonViewModel = new HoaDonViewModel
            {
                Id = hoaDon.Id,
                MaHoaDon = hoaDon.MaHoaDon,
                DatBanId = hoaDon.DatBanId,
                TenKhachHang = hoaDon.DatBan?.KhachHang?.TenKhachHang ?? "",
                SDT = hoaDon.DatBan?.KhachHang?.SDT ?? "",
                TenLienHe = hoaDon.DatBan?.TenLienHe ?? "",
                SDTLienHe = hoaDon.DatBan?.SDTLienHe ?? "",
                ThoiGianDatBan = hoaDon.DatBan?.ThoiGianDatBan ?? default(DateTime),
                ThoiGianThanhToan = hoaDon.NgayThanhToan,
                TongTienGoiMon = hoaDon.TongTienGoiMon,
                TienCoc = tienCoc,
                ThueVat = thueVat, 
                TongTienThanhToan = hoaDon.TongTienThanhToan,
                GiamGia = hoaDon.GiamGia,
                PhuongThucThanhToanDisplay = hoaDon.PhuongThuc.HasValue ? GetEnumDisplayName(hoaDon.PhuongThuc.Value) : "Chưa thanh toán",
                TrangThaiDisplay = GetEnumDisplayName(hoaDon.TrangThai),
                Bans = banNames
            };

            // Lấy danh sách phương thức thanh toán từ enum
            var paymentMethods = Enum.GetValues(typeof(PhuongThucThanhToan))
                .Cast<PhuongThucThanhToan>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = GetEnumDisplayName(e)
                }).ToList();

            ViewBag.PaymentMethods = paymentMethods;
            ViewBag.GoiMonList = goiMonViewModels;
            return View(hoaDonViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanThanhToan(int id, int phuongThuc)
        {
           
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "ID hóa đơn không hợp lệ!" });
                }

                if (!Enum.IsDefined(typeof(PhuongThucThanhToan), phuongThuc) || phuongThuc <= 0)
                {
                    return Json(new { success = false, message = "Phương thức thanh toán không hợp lệ!" });
                }
                // Lấy NhanVienId từ User Claims
                var nhanVienIdClaim = User.FindFirst("NhanVienId")?.Value;
                if (string.IsNullOrEmpty(nhanVienIdClaim) || !int.TryParse(nhanVienIdClaim, out int nhanVienId))
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin nhân viên. Vui lòng kiểm tra đăng nhập." });
                }
                var phuongThucEnum = (PhuongThucThanhToan)phuongThuc;
                await _hoaDonService.ConfirmPaymentAsync(id, phuongThucEnum, nhanVienId);
                //return Json(new { success = true, message = "Thanh toán thành công!", redirectUrl = Url.Action("LichSuHoaDon", "HoaDon", new { area = "Admin" }) });
                return Json(new { success = true, message = "Thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xác nhận thanh toán: {ex.Message}" });
            }
        }

        public async Task<IActionResult> HoaDonHienThoi(int pageIndex = 1, int pageSize = 5)
        {
            var filter = new HoaDonFilterModel
            {
                TrangThai = TrangThaiHoaDon.ChuaThanhToan
            };
            var result = await _hoaDonService.GetPagedHoaDonsAsync(pageIndex, pageSize, filter);
            var items = result.Items.Where(hd => hd.TrangThai == TrangThaiHoaDon.ChuaThanhToan).ToList();
            var totalCount = items.Count;

            var hoaDonViewModels = items.Select(hd => new HoaDonViewModel
            {
                Id = hd.Id,
                MaHoaDon = hd.MaHoaDon,
                DatBanId = hd.DatBanId,
                TenKhachHang = hd.DatBan?.KhachHang?.TenKhachHang ?? "",
                SDT = hd.DatBan?.KhachHang?.SDT ?? "",
                TenLienHe = hd.DatBan?.TenLienHe ?? "",
                SDTLienHe = hd.DatBan?.SDTLienHe ?? "",
                TongTienGoiMon = hd.TongTienGoiMon,
                GiamGia = hd.GiamGia,
                PhuongThucThanhToanDisplay = hd.PhuongThuc.HasValue ? GetEnumDisplayName(hd.PhuongThuc.Value) : "Chưa thanh toán",
                TrangThaiDisplay = GetEnumDisplayName(hd.TrangThai)
            }).ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            return View(hoaDonViewModels);
        }

        public async Task<IActionResult> LichSuHoaDon(int pageIndex = 1, int pageSize = 5)
        {
            var filter = new HoaDonFilterModel
            {
                TrangThai = TrangThaiHoaDon.DaThanhToan
            };
            var result = await _hoaDonService.GetPagedHoaDonsAsync(pageIndex, pageSize, filter);
            var items = result.Items.Where(hd => hd.TrangThai == TrangThaiHoaDon.DaThanhToan).ToList();
            var totalCount = items.Count;

            var hoaDonViewModels = items.Select(hd => new HoaDonViewModel
            {
                Id = hd.Id,
                MaHoaDon = hd.MaHoaDon,
                DatBanId = hd.DatBanId,
               
                TenKhachHang = hd.DatBan?.KhachHang?.TenKhachHang ?? "",
                SDT = hd.DatBan?.KhachHang?.SDT ?? "",
                TenLienHe = hd.DatBan?.TenLienHe ?? "",
                SDTLienHe = hd.DatBan?.SDTLienHe ?? "",
                ThoiGianThanhToan = hd.NgayThanhToan,
                TongTienGoiMon = hd.TongTienGoiMon,
                GiamGia = hd.GiamGia,
                TenNhanVien = hd.NhanVien?.TenNhanVien ?? " ",
                PhuongThucThanhToanDisplay = hd.PhuongThuc.HasValue ? GetEnumDisplayName(hd.PhuongThuc.Value) : "Chưa thanh toán",
                TrangThaiDisplay = GetEnumDisplayName(hd.TrangThai)
            }).ToList();

            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            return View(hoaDonViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedHoaDonHienThoi(int pageIndex = 1, int pageSize = 5, string tenKhachHang = "", string sdt = "", string maHoaDon = "")
        {
            try
            {
                var filter = new HoaDonFilterModel
                {
                    TenKhachHang = tenKhachHang,
                    SDT = sdt,
                    MaHoaDon = maHoaDon,
                    TrangThai = TrangThaiHoaDon.ChuaThanhToan 
                };
                var (items, totalCount) = await _hoaDonService.GetPagedHoaDonsAsync(pageIndex, pageSize, filter);

                var pagedItems = items.Select(hd => new
                {
                    id = hd.Id,
                    maHoaDon = hd.MaHoaDon,
                    tenKhachHang = hd.DatBan?.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                    sdt = hd.DatBan?.KhachHang?.SDT ?? "",
                    tenLienHe = hd.DatBan?.TenLienHe ?? "",
                    sDTLienHe = hd.DatBan?.SDTLienHe ?? "",
                    ngayThanhToan = hd.NgayThanhToan != DateTime.MinValue ? hd.NgayThanhToan.ToString("dd/MM/yyyy HH:mm") : "Chưa thanh toán",
                    tongTienGoiMon = hd.TongTienGoiMon,
                    giamGia = hd.GiamGia,
                    phuongThucThanhToanDisplay = hd.PhuongThuc.HasValue ? GetEnumDisplayName(hd.PhuongThuc.Value) : "Chưa thanh toán",
                    trangThaiDisplay = GetEnumDisplayName(hd.TrangThai),
                    bans = hd.DatBan?.DatBanBans?.Any() == true ? string.Join(", ", hd.DatBan.DatBanBans.Select(dbb => dbb.Ban.TenBan)) : ""
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
                    message = $"Lỗi khi tải dữ liệu hóa đơn: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedLichSuHoaDon(int pageIndex = 1, int pageSize = 5, string tenKhachHang = "", string sdt = "", string maHoaDon = "")
        {
            var filter = new HoaDonFilterModel
            {
                TenKhachHang = tenKhachHang,
                SDT = sdt,
                MaHoaDon = maHoaDon,
                TrangThai = TrangThaiHoaDon.DaThanhToan

            };
            var (items, totalCount) = await _hoaDonService.GetPagedHoaDonsAsync(pageIndex, pageSize, filter);

            var pagedItems = items.Select(hd => new
            {
                id = hd.Id,
                maHoaDon = hd.MaHoaDon,
                tenKhachHang = hd.DatBan?.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                sdt = hd.DatBan?.KhachHang?.SDT ?? "",
                tenLienHe = hd.DatBan?.TenLienHe ?? "",
                sDTLienHe = hd.DatBan?.SDTLienHe ?? "",
                ngayThanhToan = hd.NgayThanhToan != DateTime.MinValue ? hd.NgayThanhToan.ToString("dd/MM/yyyy HH:mm") : "Chưa thanh toán",
                tongTienGoiMon = hd.TongTienGoiMon,
                tongTienThanhToan = hd.TongTienThanhToan,
                giamGia = hd.GiamGia,
                tenNhanVien = hd.NhanVien?.TenNhanVien ?? " ",
                phuongThucThanhToanDisplay = hd.PhuongThuc.HasValue ? GetEnumDisplayName(hd.PhuongThuc.Value) : "Chưa thanh toán",
                trangThaiDisplay = GetEnumDisplayName(hd.TrangThai)
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

        [HttpGet]
        public async Task<IActionResult> DownloadHoaDonPdf(int id)
        {
            var hoaDon = await _hoaDonService.GetByIdWithDetailsAsync(id);
            if (hoaDon == null) return NotFound();
            if (hoaDon.TrangThai != TrangThaiHoaDon.DaThanhToan)
                return BadRequest("Hóa đơn chỉ xuất khi đã thanh toán.");

            var goiMons = await _goiMonService.GetByDatBanIdAsync(hoaDon.DatBanId);
            var monAns = await _monAnService.GetAllAsync();

            var goiMonViewModels = goiMons
           .GroupBy(gm => gm.MonAnId) // Gộp theo MonAnId
           .Select(g => new GoiMonViewModel
           {
               MonAnId = g.Key,
               SoLuong = g.Sum(x => x.SoLuong),
               Gia = g.First().Gia,
               TenMonAn = monAns.FirstOrDefault(m => m.Id == g.Key)?.TenMonAn ?? "Món không xác định"
           }).ToList();
            var banNames = hoaDon.DatBan?.DatBanBans?.Any() == true
                ? string.Join(", ", hoaDon.DatBan.DatBanBans.Select(dbb => dbb.Ban.TenBan))
                : "Chưa ghép bàn";

            var tienCoc = hoaDon.DatBan?.CocTien ?? 0;
            var soTienTruocVat = hoaDon.TongTienGoiMon - tienCoc;
            var thueVat = soTienTruocVat > 0 ? soTienTruocVat * VatRate : 0;
            var tongTienThanhToan = soTienTruocVat + thueVat;

            var hoaDonViewModel = new HoaDonViewModel
            {
                Id = hoaDon.Id,
                MaHoaDon = hoaDon.MaHoaDon,
                DatBanId = hoaDon.DatBanId,
                TenKhachHang = hoaDon.DatBan?.KhachHang?.TenKhachHang ?? "",
                SDT = hoaDon.DatBan?.KhachHang?.SDT ?? "",
                TenLienHe = hoaDon.DatBan?.TenLienHe ?? "",
                SDTLienHe = hoaDon.DatBan?.SDTLienHe ?? "",
                ThoiGianDatBan = hoaDon.DatBan?.ThoiGianDatBan ?? default(DateTime),
                ThoiGianThanhToan = hoaDon.NgayThanhToan,
                TongTienGoiMon = hoaDon.TongTienGoiMon,
                TienCoc = tienCoc,
                ThueVat = thueVat,
                TongTienThanhToan = tongTienThanhToan,
                GiamGia = hoaDon.GiamGia,
                PhuongThucThanhToanDisplay = hoaDon.PhuongThuc.HasValue ? GetEnumDisplayName(hoaDon.PhuongThuc.Value) : "Chưa thanh toán",
                TrangThaiDisplay = GetEnumDisplayName(hoaDon.TrangThai),
                Bans = banNames
            };

            var pdfViewModel = new HoaDonPdfViewModel { HoaDon = hoaDonViewModel, GoiMons = goiMonViewModels };
            var htmlContent = await RenderViewToStringAsync("PrintHoaDon", pdfViewModel);

            var globalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A6,
                Orientation = Orientation.Portrait,
                Margins = new MarginSettings { Top = 5, Bottom = 5, Left = 5, Right = 5 },
                DocumentTitle = $"HoaDon_{hoaDon.MaHoaDon}"
            };

            var objectSettings = new ObjectSettings
            {
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                PagesCount = true
            };

            var pdf = new HtmlToPdfDocument { GlobalSettings = globalSettings, Objects = { objectSettings } };
            try
            {
                var pdfBytes = _converter.Convert(pdf);
                return File(pdfBytes, "application/pdf", $"HoaDon_{hoaDon.MaHoaDon}.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message} - StackTrace: {ex.StackTrace}");
                return BadRequest($"Lỗi xuất PDF: {ex.Message}");
            }
        }

        private async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            ViewData.Model = model;
            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
            if (viewResult.View == null) throw new FileNotFoundException($"View {viewName} không tìm thấy.");
            var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            return sw.GetStringBuilder().ToString();
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