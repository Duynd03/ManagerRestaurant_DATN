using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.DanhMucService;
using QuanLyNhaHang_DATN.Services.MonAnService;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MonAnController : Controller
    {
        private readonly IMonAnService _monAnService;
        private readonly IDanhMucService _danhMucService;
        private readonly IWebHostEnvironment _env;

        public MonAnController(IMonAnService monAnService, IDanhMucService danhMucService, IWebHostEnvironment env)
        {
            _monAnService = monAnService;
            _danhMucService = danhMucService;
            _env = env;

        }
       
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 5, string? tenMonAn = null, int? danhMucId = null, int? trangThai = null)
        {
            // Tạo filter từ các tham số đầu vào
            var filter = new MonAnFilterModel
            {
                TenMonAn = tenMonAn,
                DanhMucId = danhMucId,
                TrangThai = trangThai
            };
            var result = await _monAnService.GetPagedAsync(pageIndex, pageSize, filter);

            var danhMucs = await _danhMucService.GetAllAsync();

            ViewBag.SearchTenMonAn = tenMonAn;
            ViewBag.SelectedDanhMucId = danhMucId;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.DanhMucList = danhMucs;

            return View(result.Items);
        }
        
        public async Task<IActionResult> GetPagedData(int pageIndex = 1, int pageSize = 5, string? tenMonAn = null, int? danhMucId = null, int? trangThai = null)
        {
            var filter = new MonAnFilterModel
            {
                TenMonAn = tenMonAn,
                DanhMucId = danhMucId,
                TrangThai = trangThai
            };

            var result = await _monAnService.GetPagedAsync(pageIndex, pageSize, filter);

            var items = result.Items.Select(item => new
            {
                Id = item.Id, 
                item.TenMonAn,
                item.MoTa, // Thêm Mô tả
                item.Gia,
                item.HinhAnh,
                TenDanhMuc = item.DanhMuc?.TenDanhMuc, 
                TrangThaiDisplay = GetEnumDisplayName(item.TrangThai),
                 TrangThaiValue = (int)item.TrangThai // Thêm giá trị số của TrangThai
            });

            return Json(new
            {
                items,
                totalCount = result.TotalCount,
                pageIndex,
                pageSize,
                search = tenMonAn
            });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //ViewBag.DanhMucs = await _danhMucService.GetAllAsync();
            var danhMucs = await _danhMucService.GetAllAsync();
            ViewBag.DanhMucs = new SelectList(danhMucs, "Id", "TenDanhMuc");

            var trangThaiList = Enum.GetValues(typeof(TrangThaiMonAn))
                .Cast<TrangThaiMonAn>()   //chuyển đổi sang kiểu TrangThaiMonAn
                .Select(e => new SelectListItem
            {
            Value = ((int)e).ToString(), // Giá trị enum (0, 1)
            Text = GetEnumDisplayName(e) // Lấy DisplayName (Hết hàng, Có sẵn)
            })
            .ToList();
            ViewBag.TrangThaiList = trangThaiList;
            return PartialView("_CreatePartial", new MonAn());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] MonAn monAn, IFormFile? imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    ModelState.AddModelError("imageFile", "Vui lòng chọn hình ảnh");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                monAn.HinhAnh = await SaveImageAsync(imageFile);
                await _monAnService.AddAsync(monAn);
                return Json(new { success = true, message = "Thêm món ăn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var monAn = await _monAnService.GetByIdAsync(id);
            if (monAn == null) return NotFound();

            ViewBag.DanhMucs = await _danhMucService.GetAllAsync();
         
            // Tạo danh sách trạng thái sử dụng GetEnumDisplayName
            var trangThaiList = Enum.GetValues(typeof(TrangThaiMonAn))
                .Cast<TrangThaiMonAn>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = GetEnumDisplayName(e)
                })
                .ToList();
            ViewBag.TrangThaiList = trangThaiList;
            // Thêm ViewBag.DefaultImage để JavaScript sử dụng
            ViewBag.DefaultImage = monAn.HinhAnh != null ? $"/uploads/{monAn.HinhAnh}" : "";
            return PartialView("_EditPartial", monAn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] MonAn monAn, IFormFile? imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                var existing = await _monAnService.GetByIdAsync(monAn.Id);
                if (existing == null) return NotFound();

                existing.TenMonAn = monAn.TenMonAn;
                existing.MoTa = monAn.MoTa;
                existing.Gia = monAn.Gia;
                existing.DanhMucId = monAn.DanhMucId;
                existing.TrangThai = monAn.TrangThai;
                if (imageFile != null)
                {
                    existing.HinhAnh = await SaveImageAsync(imageFile);
                }

                await _monAnService.UpdateAsync(existing);
                return Json(new { success = true, message = "Cập nhật món ăn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _monAnService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xóa: {ex.Message}" });
            }
        }
        private async Task<string?> SaveImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var fileName = Path.GetFileName(file.FileName); 
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName; 
        }
        private string GetEnumDisplayName(TrangThaiMonAn value)
        {
            // Lấy thuộc tính [Display(Name="...")]
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
