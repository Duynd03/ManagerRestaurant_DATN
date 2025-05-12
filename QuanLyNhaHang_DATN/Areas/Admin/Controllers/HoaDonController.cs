using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHang_DATN.Areas.Admin.Controllers
{
    public class HoaDonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
