using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.BanService;
using QuanLyNhaHang_DATN.Services.DanhMucService;
using QuanLyNhaHang_DATN.Services.KhuVucBanService;
using QuanLyNhaHang_DATN.Services.MonAnService;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BanController : Controller
    {
        private readonly IBanService _banService;
        private readonly IKhuVucBanService _khuVucBanService;
        public BanController(IBanService banService, IKhuVucBanService khuVucBanService)
        {
            _banService = banService;
            _khuVucBanService = khuVucBanService;

        }
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null, int? trangThai = null)
        {
            var filter = new BanFilterModel
            {
                TenBan = tenBan,
                KhuVucBanId = khuVucBanId,
               
            };
            var result = await _banService.GetPagedAsync(pageIndex, pageSize, filter);
            var khuVucBans = await _khuVucBanService.GetAllAsync();
            ViewBag.SearchTenBan = tenBan;
            ViewBag.SelectedKhuVucBanId = khuVucBanId;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.KhuVucBanList = khuVucBans;

            return View(result.Items);
        }
        //private string GetEnumDisplayName(TrangThaiBan value)
        //{
        //    // Lấy thuộc tính [Display(Name="...")]
        //    var memberInfo = value.GetType().GetMember(value.ToString());
        //    if (memberInfo != null && memberInfo.Length > 0)
        //    {
        //        var attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
        //        if (attrs != null && attrs.Length > 0)
        //            return ((DisplayAttribute)attrs[0]).Name;
        //    }
        //    return value.ToString();
        //}
        public async Task<IActionResult> GetPagedData(int pageIndex = 1, int pageSize = 5, string? tenBan = null, int? khuVucBanId = null)
        {
            var filter = new BanFilterModel
            {
                TenBan = tenBan,
                KhuVucBanId = khuVucBanId
              
            };

            var result = await _banService.GetPagedAsync(pageIndex, pageSize, filter);

            var items = result.Items.Select(item => new
            {
                Id = item.Id,
                TenBan = item.TenBan,
                TenKhuVucBan = item.KhuVucBan?.TenKhuVuc,
                //TrangThaiDisplay = GetEnumDisplayName(item.TrangThai),
                //TrangThaiValue = (int)item.TrangThai // Thêm giá trị số của TrangThai
            });

            return Json(new
            {
                items,
                totalCount = result.TotalCount,
                pageIndex,
                pageSize,
                search = tenBan
            });
        }
        public async Task<IActionResult> Create()
        {
            var khuVucBans = await _khuVucBanService.GetAllAsync();
            ViewBag.KhuVucBans = new SelectList(khuVucBans, "Id", "TenKhuVuc");
            //var trangThaiList = Enum.GetValues(typeof(TrangThaiBan))
            //    .Cast<TrangThaiBan>()
            //    .Select(e => new SelectListItem
            //    {
            //        Value = ((int)e).ToString(),
            //        Text = GetEnumDisplayName(e)
            //    })
            //.ToList();
            //ViewBag.TrangThaiList = trangThaiList;
            return PartialView("_CreatePartial", new Ban());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Ban ban)
        {
            try
            {
                
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                await _banService.AddAsync(ban);
                return Json(new { success = true, message = "Thêm Bàn thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ban = await _banService.GetByIdAsync(id);
            if (ban == null) return NotFound();

            ViewBag.KhuVucBans = await _khuVucBanService.GetAllAsync();

            //var trangThaiList = Enum.GetValues(typeof(TrangThaiBan))
            //    .Cast<TrangThaiBan>()
            //    .Select(e => new SelectListItem
            //    {
            //        Value = ((int)e).ToString(),
            //        Text = GetEnumDisplayName(e)
            //    })
            //    .ToList();
            //ViewBag.TrangThaiList = trangThaiList;
            return PartialView("_EditPartial", ban);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] Ban ban)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                var existing = await _banService.GetByIdAsync(ban.Id);
                if (existing == null) return NotFound();

                existing.TenBan = ban.TenBan;
                existing.KhuVucBanId = ban.KhuVucBanId;
                //existing.TrangThai = ban.TrangThai;
                await _banService.UpdateAsync(existing);
                return Json(new { success = true, message = "Cập nhật Bàn thành công" });
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
                await _banService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xóa: {ex.Message}" });
            }
        }
    }
}
