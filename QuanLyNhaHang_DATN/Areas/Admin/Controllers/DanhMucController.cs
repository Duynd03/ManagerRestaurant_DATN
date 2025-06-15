using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.DanhMucService;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DanhMucController : Controller
    {

        private readonly IDanhMucService _danhMucService;

        public DanhMucController(IDanhMucService danhMucService)
        {
            _danhMucService = danhMucService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 5, string? tenDanhMuc = null)
        {
            // Tạo filter từ các tham số đầu vào
            var filter = new DanhMucFilterModel
            {
               TenDanhMuc = tenDanhMuc
            };
            var result = await _danhMucService.GetPagedAsync(pageIndex, pageSize, filter);


            ViewBag.SearchTenDanhMuc = tenDanhMuc;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;
            return View(result.Items);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedData(int pageIndex = 1, int pageSize = 5, string? tenDanhMuc = null)
        {
            var filter = new DanhMucFilterModel
            {
                TenDanhMuc = tenDanhMuc
            };

            var result = await _danhMucService.GetPagedAsync(pageIndex, pageSize, filter);

            var items = result.Items.Select(item => new
            {
                Id = item.Id,
                item.TenDanhMuc,
                item.MoTa, 

            });

            return Json(new
            {
                items,
                totalCount = result.TotalCount,
                pageIndex,
                pageSize,
                search = tenDanhMuc
            });
        } 
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreatePartial", new DanhMuc());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] DanhMuc danhMuc)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            await _danhMucService.AddAsync(danhMuc);
            return Json(new { success = true, message = "Thêm danh mục thành công" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var danhMuc = await _danhMucService.GetByIdAsync(id);
            if (danhMuc == null) return NotFound();

            return PartialView("_EditPartial", danhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] DanhMuc danhMuc)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
            }

            var existing = await _danhMucService.GetByIdAsync(danhMuc.Id);
            if (existing == null) return NotFound();

            existing.TenDanhMuc = danhMuc.TenDanhMuc;
            existing.MoTa = danhMuc.MoTa;
            await _danhMucService.UpdateAsync(existing);

            return Json(new { success = true, message = "Cập nhật danh mục thành công" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _danhMucService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xóa: {ex.Message}" });
            }
        }
    }
}

