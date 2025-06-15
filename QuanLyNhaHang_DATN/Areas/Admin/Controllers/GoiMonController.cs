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
using QuanLyNhaHang_DATN.Services.HoaDonService;
using QuanLyNhaHang_DATN.Services.DatBanService;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class GoiMonController : Controller
    {
        private readonly IDanhMucService _danhMucService;
        private readonly IMonAnService _monAnService;
        private readonly IGoiMonService _goiMonService;
        private readonly IHoaDonService _hoaDonService;
        private readonly IDatBanService _datBanService;
        public GoiMonController(IDanhMucService danhMucService, IMonAnService monAnService, IGoiMonService goiMonService, IHoaDonService hoaDonService, IDatBanService datBanService)
        {
            _danhMucService = danhMucService;
            _monAnService = monAnService;
            _goiMonService = goiMonService;
            _hoaDonService = hoaDonService;
            _datBanService = datBanService;
        }


        public async Task<IActionResult> Index(int datBanId)
        {
            var danhMucs = (await _danhMucService.GetAllAsync()).ToList();
            ViewBag.DanhMucList = danhMucs;
            ViewBag.DatBanId = datBanId;

            var maxLanGoiMon = await _goiMonService.GetMaxLanGoiMonAsync(datBanId);
            var allGoiMons = await _goiMonService.GetByDatBanIdAsync(datBanId);
            var monAns = await _monAnService.GetAllAsync();
            var goiMonByLan = new Dictionary<int, List<GoiMonViewModel>>();

            for (int lan = 1; lan <= maxLanGoiMon; lan++)
            {
                var goiMons = await _goiMonService.GetByDatBanIdAndLanGoiMonAsync(datBanId, lan);
                goiMonByLan[lan] = goiMons.Select(gm => new GoiMonViewModel
                {
                    MonAnId = gm.MonAnId,
                    SoLuong = gm.SoLuong,
                    Gia = gm.Gia,
                    TenMonAn = monAns.FirstOrDefault(m => m.Id == gm.MonAnId)?.TenMonAn ?? ""
                }).ToList();
            }

            ViewBag.GoiMonByLan = goiMonByLan;
            ViewBag.MaxLanGoiMon = maxLanGoiMon;
            ViewBag.CurrentLanGoiMon = maxLanGoiMon + 1; // Truyền lần gọi món mới

            return View(new DatBan { Id = datBanId });
        }

        // Lấy danh sách món ăn theo danh mục
        [HttpGet]
        public async Task<IActionResult> GetMonAnByDanhMuc(int danhMucId, int pageIndex = 1, int pageSize = 5)
        {
            var filter = new QuanLyNhaHang_DATN.Areas.Admin.Models.MonAnFilterModel
            {
                DanhMucId = danhMucId, 
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

        // Lưu danh sách gọi món
        [HttpPost]
        public async Task<IActionResult> SaveGoiMon(SaveGoiMonRequest request)
        {
            if (request == null || request.goiMonViewModels == null || !request.goiMonViewModels.Any())
            {
                return Json(new { success = false, message = "Danh sách gọi món trống." });
            }

            try
            {
                var monAns = await _monAnService.GetAllAsync();
                var maxLanGoiMon = await _goiMonService.GetMaxLanGoiMonAsync(request.datBanId);
                var lanGoiMon = maxLanGoiMon + 1; // Tăng lần gọi món cho món mới
                var goiMonList = request.goiMonViewModels.Select(vm => new GoiMon
                {
                    MonAnId = vm.MonAnId,
                    SoLuong = vm.SoLuong,
                    Gia = vm.Gia,
                    DatBanId = request.datBanId,
                    ThoiGianGoiMon = DateTime.Now,
                    LanGoiMon = lanGoiMon // Gán lần gọi món mới
                }).ToList();

                await _goiMonService.SaveGoiMonListAsync(request.datBanId, goiMonList, lanGoiMon);
                await _datBanService.UpdateBanGoiMonAsync(request.datBanId);
                return Json(new { success = true, message = "Lưu gọi món thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi lưu gọi món: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetLatestGoiMonAsync(int datBanId)
        {
            if (datBanId <= 0)
            {
                return Json(new { success = false, message = "Đặt bàn không hợp lệ!" });
            }

            try
            {
                var maxLanGoiMon = await _goiMonService.GetMaxLanGoiMonAsync(datBanId);
                var savedGoiMons = await _goiMonService.GetByDatBanIdAndLanGoiMonAsync(datBanId, maxLanGoiMon);
                var savedItems = savedGoiMons.Select(gm => new
                {
                    MonAnId = gm.MonAnId,
                    SoLuong = gm.SoLuong,
                    Gia = gm.Gia,
                    TenMonAn = gm.MonAn?.TenMonAn ?? ""
                }).ToList();

                return Json(new { success = true, savedItems, message = "Đã tải danh sách món đã lưu!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xử lý: {ex.Message}" });
            }
        }
    }
}
