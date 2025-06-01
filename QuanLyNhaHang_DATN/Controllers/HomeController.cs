using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.DanhMucService;
using QuanLyNhaHang_DATN.Services.MonAnService;
using QuanLyNhaHang_DATN.ViewModels;
using System.Diagnostics;

namespace QuanLyNhaHang_DATN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMonAnService _monAnService;
        private readonly IDanhMucService _danhMucService;
        public HomeController(IDanhMucService danhMucService, IMonAnService monAnService)
        {
            _danhMucService = danhMucService;
            _monAnService = monAnService;
        }
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        } 
        public IActionResult Contact()
        {
            return View();
        }
        public async Task<IActionResult> Menu()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories(int pageIndex = 1, int pageSize = 100)
        {
            try
            {
                var filter = new DanhMucFilterModel();
                var result = await _danhMucService.GetPagedAsync(pageIndex, pageSize, filter);

                if (result.Items == null || !result.Items.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy danh mục nào." });
                }

                var items = result.Items.Select(item => new
                {
                    Id = item.Id,
                    item.TenDanhMuc,
                    item.MoTa
                }).ToList();

                return Json(new
                {
                    success = true,
                    items,
                    totalCount = result.TotalCount,
                    pageIndex,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi tải danh mục: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDishesByCategory(int? danhMucId, int pageIndex = 1, int pageSize = 8)
        {
            try
            {
                var filter = new MonAnFilterModel
                {
                    DanhMucId = danhMucId,
                    TrangThai = (int?)TrangThaiMonAn.CoSan // Chỉ lấy món ăn có sẵn
                };
                var result = await _monAnService.GetPagedAsync(pageIndex, pageSize, filter);

                if (result.Items == null || !result.Items.Any())
                {
                    return Json(new { success = false, message = "Không tìm thấy món ăn nào." });
                }

                var items = result.Items.Select(item => new
                {
                    Id = item.Id,
                    item.TenMonAn,
                    item.MoTa,
                    item.Gia,
                    item.HinhAnh,
                    TenDanhMuc = item.DanhMuc?.TenDanhMuc
                }).ToList();

                return Json(new
                {
                    success = true,
                    items,
                    totalCount = result.TotalCount,
                    pageIndex,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi tải món ăn: " + ex.Message });
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
