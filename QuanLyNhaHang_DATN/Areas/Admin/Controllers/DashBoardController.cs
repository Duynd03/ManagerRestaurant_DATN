using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Services.DashBoardService;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashBoardService;

        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        public async Task<IActionResult> Index(string filterType = "daily", string fromDate = null, string toDate = null, int? month = null, int? year = null, int? compareYear = null)
        {
            var model = await _dashBoardService.GetDashBoardDataAsync(filterType, fromDate, toDate, month, year, compareYear);
            return View(model);
        }
    }
}