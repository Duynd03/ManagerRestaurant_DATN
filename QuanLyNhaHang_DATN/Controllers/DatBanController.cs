using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using QuanLyNhaHang_DATN.Hubs;
using QuanLyNhaHang_DATN.Services.DatBanService;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using QuanLyNhaHang_DATN.Services.VNPay;
using QuanLyNhaHang_DATN.ViewModels;
using StackExchange.Redis;
using System.Text.Json;

namespace QuanLyNhaHang_DATN.Controllers
{
    public class DatBanController : Controller
    {
        private readonly IDatBanService _datBanService;
        private readonly IKhachHangService _khachHangService;
        private readonly IVNPayService _vnPayService;
        private readonly IHubContext<DatBanHub> _hubContext;
        private readonly IDistributedCache _cache;

        public DatBanController(IDatBanService datBanService, IKhachHangService khachHangService, IVNPayService vnPayService, IHubContext<DatBanHub> hubContext, IDistributedCache cache)
        {
            _datBanService = datBanService;
            _khachHangService = khachHangService;
            _vnPayService = vnPayService;
            _hubContext = hubContext;
            _cache = cache;
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

                var tempTransactionId = Guid.NewGuid().ToString();

                // Lưu vào Redis
                var datBanJson = JsonSerializer.Serialize(datBan);
                await _cache.SetStringAsync($"TempDatBan:{tempTransactionId}", datBanJson, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

                var paymentUrl = await _vnPayService.CreatePaymentUrl(datBan, HttpContext, tempTransactionId);

                return Json(new { success = true, message = "Vui lòng hoàn tất thanh toán", tempTransactionId, paymentUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

                var tempTransactionId = Guid.NewGuid().ToString();

                var datBanJson = JsonSerializer.Serialize(datBan);
                await _cache.SetStringAsync($"TempDatBan:{tempTransactionId}", datBanJson, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                });

                var paymentUrl = await _vnPayService.CreatePaymentUrl(datBan, HttpContext, tempTransactionId);

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
            //await _hubContext.Clients.Group("Admins").SendAsync("ReceiveDatBanUpdate", datBan);
            //await _hubContext.Clients.All.SendAsync("ReceivePaymentNotification", new
            //{
            //    Success = true,
            //    DatBanId = datBan.Id,
            //    Message = "Thanh toán thành công"
            //});
            //await _hubContext.Clients.Group("Admins").SendAsync("ReceiveDatBanUpdate", new
            //{
            //    datBanId = datBan.Id,
            //    message = $"Bạn có đơn đặt bàn mới Id {datBan.Id}",
            //    time = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
            //});
            var notification = new
            {
                datBanId = datBan.Id,
                message = $"Bạn có đơn đặt bàn mới Id {datBan.Id}",
                time = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
            };

            // Lưu thông báo vào Redis với key chung
            var notificationKey = "AdminNotifications";
            var existingNotificationsJson = await _cache.GetStringAsync(notificationKey);
            var notifications = string.IsNullOrEmpty(existingNotificationsJson)
                ? new List<object>()
                : JsonSerializer.Deserialize<List<object>>(existingNotificationsJson);
            notifications.Add(notification);

            await _cache.SetStringAsync(notificationKey, JsonSerializer.Serialize(notifications), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) // Lưu 1 ngày
            });

            // Gửi thông báo qua SignalR cho admin
            await _hubContext.Clients.Group("Admins").SendAsync("ReceiveBookingNotification", new
            {
                DatBanId = datBan.Id,
                Message = $"Bạn có đơn đặt bàn mới ID {datBan.Id} từ thanh toán online lúc {DateTime.Now:HH:mm dd/MM/yyyy}",
                Time = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
            }, "NEW");

            await _hubContext.Clients.All.SendAsync("ReceivePaymentNotification", new
            {
                Success = true,
                DatBanId = datBan.Id,
                Message = "Thanh toán thành công"
            });

            return RedirectToAction("PaymentSuccess", new { datBanId = datBan.Id });
        }
        [HttpGet]
        public async Task<IActionResult> GetNotificationsFromRedis()
        {
            var notificationKey = "AdminNotifications";
            var notificationsJson = await _cache.GetStringAsync(notificationKey);
            var notifications = string.IsNullOrEmpty(notificationsJson)
                ? new List<object>()
                : JsonSerializer.Deserialize<List<object>>(notificationsJson);
            return Json(new { success = true, data = notifications });
        }

        [HttpPost]
        public async Task<IActionResult> ClearNotificationsFromRedis()
        {
            var notificationKey = "AdminNotifications";
            await _cache.RemoveAsync(notificationKey);
            return Json(new { success = true });
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