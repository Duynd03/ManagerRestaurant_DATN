using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHang_DATN.Models;
using System.Diagnostics;

namespace QuanLyNhaHang_DATN.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

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
        public IActionResult Menu()
        {
            return View();
        }
        public IActionResult Reservation()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
