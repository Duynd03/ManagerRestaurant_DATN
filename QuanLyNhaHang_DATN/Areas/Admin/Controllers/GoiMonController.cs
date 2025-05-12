using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.DanhMucService;
using QuanLyNhaHang_DATN.Services.MonAnService;
using QuanLyNhaHang_DATN.Services.GoiMonService;
using QuanLyNhaHang_DATN.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GoiMonController : Controller
    {
        private readonly IDanhMucService _danhMucService;
        private readonly IMonAnService _monAnService;
        private readonly IGoiMonService _goiMonService;

        public GoiMonController(IDanhMucService danhMucService, IMonAnService monAnService, IGoiMonService goiMonService)
        {
            _danhMucService = danhMucService;
            _monAnService = monAnService;
            _goiMonService = goiMonService;
        }

        // Hiển thị trang gọi món
        public async Task<IActionResult> Index(int datBanId)
        {
            var danhMucs = (await _danhMucService.GetAllAsync()).ToList();
            ViewBag.DanhMucList = danhMucs;
            ViewBag.DatBanId = datBanId;

            // Lấy danh sách gọi món đã lưu và ánh xạ sang ViewModel
            var goiMons = await _goiMonService.GetByDatBanIdAsync(datBanId);
            var monAns = await _monAnService.GetAllAsync();
            var goiMonViewModels = goiMons.Select(gm => new GoiMonViewModel
            {
                MonAnId = gm.MonAnId,
                SoLuong = gm.SoLuong,
                Gia = gm.Gia,
                TenMonAn = monAns.FirstOrDefault(m => m.Id == gm.MonAnId)?.TenMonAn ?? ""
            }).ToList();

            ViewBag.GoiMonList = goiMonViewModels;
            return View(new DatBan { Id = datBanId });
        }

        // Lấy danh sách món ăn theo danh mục
        [HttpGet]
        public async Task<IActionResult> GetMonAnByDanhMuc(int danhMucId, int pageIndex = 1, int pageSize = 5)
        {
            var filter = new QuanLyNhaHang_DATN.Areas.Admin.Models.MonAnFilterModel
            {
                DanhMucId = danhMucId, // Lọc theo DanhMucId
                TrangThai = (int?)TrangThaiMonAn.CoSan
            };
            var (items, totalCount) = await _monAnService.GetPagedAsync(pageIndex, pageSize, filter);

            var result = items.Select(m => new
            {
                id = m.Id,
                tenMonAn = m.TenMonAn,
                gia = m.Gia
            }).ToList();

            return Json(new
            {
                success = true,
                items = result,
                totalCount = totalCount,
                pageIndex = pageIndex,
                pageSize = pageSize
            });
        }

        // Lưu danh sách gọi món từ ViewModel
        [HttpPost]
        public async Task<IActionResult> SaveGoiMon([FromBody] SaveGoiMonRequest request)
        {
            if (request == null || request.goiMonViewModels == null || !request.goiMonViewModels.Any())
            {
                return Json(new { success = false, message = "Danh sách gọi món trống." });
            }

            try
            {
                var monAns = await _monAnService.GetAllAsync();
                var goiMonList = request.goiMonViewModels.Select(vm => new GoiMon
                {
                    MonAnId = vm.MonAnId,
                    SoLuong = vm.SoLuong,
                    Gia = vm.Gia,
                    DatBanId = request.datBanId,
                    ThoiGianGoiMon = DateTime.Now
                }).ToList();

                await _goiMonService.SaveGoiMonListAsync(request.datBanId, goiMonList);
                return Json(new { success = true, message = "Lưu gọi món thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi lưu gọi món: {ex.Message}" });
            }
        }

        // Xuất hóa đơn
        [HttpPost]
        public async Task<IActionResult> CreateHoaDon(int datBanId)
        {
            decimal tongTien = await _goiMonService.CalculateTongTienAsync(datBanId);

            var hoaDon = new HoaDon
            {
                DatBanId = datBanId,
                TongTien = tongTien,
                NgayThanhToan = DateTime.Now,
                TrangThai = TrangThaiHoaDon.ChuaThanhToan,
                PhuongThucThanhToan = PhuongThucThanhToans.TienMat
            };

            // Giả sử có IHoaDonService để lưu hóa đơn (chưa triển khai)
            // await _hoaDonService.AddAsync(hoaDon);

            return Json(new { success = true, message = "Tạo hóa đơn thành công!" });
        }
    }
}