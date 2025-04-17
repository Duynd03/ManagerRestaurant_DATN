using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SideBar()
        {
            return View();
        }
        public IActionResult IndexLayout()
        {
            return View();
        }
    }
}
