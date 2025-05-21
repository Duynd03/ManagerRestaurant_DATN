using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuanLyNhaHang_DATN.Hubs;
using QuanLyNhaHang_DATN.Services.DatBanService;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using QuanLyNhaHang_DATN.Services.VNPay;
using QuanLyNhaHang_DATN.ViewModels;

namespace QuanLyNhaHang_DATN.Controllers
{
    public class DatBanController : Controller
    {
        private readonly IDatBanService _datBanService;
        private readonly IKhachHangService _khachHangService;
        private readonly IVNPayService _vnPayService;
        private readonly IHubContext<DatBanHub> _hubContext;

        public DatBanController(IDatBanService datBanService, IKhachHangService khachHangService, IVNPayService vnPayService, IHubContext<DatBanHub> hubContext)
        {
            _datBanService = datBanService;
            _khachHangService = khachHangService;
            _vnPayService = vnPayService;
            _hubContext = hubContext;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax([FromBody] DatBanViewModel datBan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                var username = User.Identity.IsAuthenticated ? User.Identity.Name : null;
                var result = await _datBanService.CreateDatBanAsync(datBan, username);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.Message, errors = result.Errors });
                }
                // Gửi thông tin đặt bàn qua SignalR đến nhóm Admins
                await _hubContext.Clients.Group("Admins").SendAsync("ReceiveDatBanUpdate", result.Data);

                return Json(new { success = true, message = result.Message, datBanId = result.Data.Id });
            }
            catch (Exception ex)
            {
                 Console.WriteLine(ex.ToString()); // Ghi log lỗi
                return StatusCode(500, new { success = false, message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var username = User.Identity.Name;
            var khachHang = await _khachHangService.GetByTaiKhoanUsernameAsync(username);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    tenKhachHang = khachHang.TenKhachHang,
                    sdt = khachHang.SDT
                }
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment([FromBody] DatBanViewModel datBan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors });
                }

                // Gọi VNPayService để tạo URL thanh toán
                var paymentUrl = await _vnPayService.CreatePaymentUrl(datBan, HttpContext);

                return Json(new { success = true, paymentUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { success = false, message = "Lỗi server", error = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentCallback()
        {
            var query = HttpContext.Request.Query;
            var (success, datBan) = await _vnPayService.ProcessCallback(query, "return");

            if (!success || datBan == null)
            {
                // Gửi thông báo qua SignalR cho người dùng
                await _hubContext.Clients.All.SendAsync("ReceivePaymentNotification", new
                {
                    Success = false,
                    Message = "Thanh toán không thành công"
                });

                return RedirectToAction("PaymentFailed", new { message = "Thanh toán không thành công." });
            }
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveDatBanUpdate", datBan);
            await _hubContext.Clients.All.SendAsync("ReceivePaymentNotification", new
            {
                Success = true,
                DatBanId = datBan.Id,
                Message = "Thanh toán thành công"
            });

            return RedirectToAction("PaymentSuccess", new { datBanId = datBan.Id });
        }

        [HttpGet]
        public IActionResult PaymentSuccess(int datBanId)
        {
            ViewBag.DatBanId = datBanId;
            return View();
        }

        [HttpGet]
        public IActionResult PaymentFailed(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IPNCallback()
        {
            var query = HttpContext.Request.Query;
            var (success, datBan) = await _vnPayService.ProcessCallback(query, "ipn");

            if (!success || datBan == null)
            {
                return Json(new { RspCode = "01", Message = "Transaction failed" });
            }

            return Json(new { RspCode = "00", Message = "Transaction successful" });
        }
    }
}