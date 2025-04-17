using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.DanhMucService;
using QuanLyNhaHang_DATN.Services.KhuVucBanService;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhuVucBanController : Controller
    {
        private readonly IKhuVucBanService _khuVucBanService;
        public KhuVucBanController(IKhuVucBanService khuVucBanService)
        {
            _khuVucBanService = khuVucBanService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var khuVucList = await _khuVucBanService.GetAllAsync();
            return View(khuVucList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new KhuVucBan());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] KhuVucBan khuVuc)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            await _khuVucBanService.AddAsync(khuVuc);
            return Json(new { success = true, message = "Thêm khu vực bàn thành công" });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var khuVuc = await _khuVucBanService.GetByIdAsync(id);
            if (khuVuc == null) return NotFound();

            return PartialView("_EditPartial", khuVuc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] KhuVucBan khuVuc)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            var existing = await _khuVucBanService.GetByIdAsync(khuVuc.Id);
            if (existing == null) return NotFound();

            existing.TenKhuVuc = khuVuc.TenKhuVuc;
            existing.MoTa = khuVuc.MoTa;
            await _khuVucBanService.UpdateAsync(existing);

            return Json(new { success = true, message = "Cập nhật khu vực bàn thành công" });
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _khuVucBanService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xóa: {ex.Message}" });
            }
        }

    }
}
