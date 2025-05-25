using Microsoft.Extensions.Options;
using QuanLyNhaHang_DATN.Config;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.ViewModels;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;

namespace QuanLyNhaHang_DATN.Services.VNPay
{
    public class VNPayService : IVNPayService
    {
        private readonly IOptions<VNPayConfig> _vnpayConfig;
        private readonly AppDbContext _context;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _cache;
        private readonly ILogger<VNPayService> _logger;

        public VNPayService(IOptions<VNPayConfig> vnpayConfig, AppDbContext context, IDistributedCache cache, ILogger<VNPayService> logger)
        {
            _vnpayConfig = vnpayConfig;
            _context = context;
            _cache = cache;
            _logger = logger;
        }
        public async Task<string> CreatePaymentUrl(DatBanViewModel model, HttpContext httpContext, string tempTransactionId = null)
        {
            tempTransactionId ??= Guid.NewGuid().ToString();

            // Lưu vào Redis
            var datBanJson = JsonSerializer.Serialize(model);
            await _cache.SetStringAsync($"TempDatBan:{tempTransactionId}", datBanJson, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            var vnpay = _vnpayConfig.Value;
            var vnp_Amount = (long)(model.CocTien * 100);
            var vnp_OrderInfo = $"Thanh toan tien coc dat ban {tempTransactionId}";
            var vnp_IpAddr = httpContext.Connection.RemoteIpAddress?.ToString();
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_ExpireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

            var inputData = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_TmnCode", vnpay.TmnCode },
                { "vnp_Amount", vnp_Amount.ToString() },
                { "vnp_Command", "pay" },
                { "vnp_CreateDate", vnp_CreateDate },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", vnp_IpAddr },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", vnp_OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", vnpay.ReturnUrl },
                { "vnp_TxnRef", tempTransactionId },
                { "vnp_ExpireDate", vnp_ExpireDate }
            };

            var queryString = string.Join("&", inputData.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
            var secureHash = HmacSha512(queryString, vnpay.HashSecret);
            return $"{vnpay.BaseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public async Task<(bool Success, DatBan DatBan)> ProcessCallback(IQueryCollection query, string ipnType)
        {
            var vnpay = _vnpayConfig.Value;
            var secureHash = query["vnp_SecureHash"];
            var tempTransactionId = query["vnp_TxnRef"];
            var inputData = new SortedDictionary<string, string>();

            _logger.LogInformation("Processing VNPay callback with query: {Query}", query.ToString());

            foreach (var (key, value) in query)
            {
                if (key.StartsWith("vnp_") && key != "vnp_SecureHash")
                {
                    inputData[key] = value;
                }
            }

            var queryString = string.Join("&", inputData.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
            var verifyHash = HmacSha512(queryString, vnpay.HashSecret);

            if (verifyHash != secureHash)
            {
                _logger.LogError("Invalid SecureHash. Expected: {VerifyHash}, Received: {SecureHash}", verifyHash, secureHash);
                return (false, null);
            }

            var responseCode = query["vnp_ResponseCode"];
            var amount = decimal.Parse(query["vnp_Amount"]) / 100;

            if (responseCode != "00")
            {
                _logger.LogWarning("Payment failed with ResponseCode: {ResponseCode}", responseCode);
                return (false, null);
            }

            // Lấy từ Redis
            var datBanJson = await _cache.GetStringAsync($"TempDatBan:{tempTransactionId}");
            if (string.IsNullOrEmpty(datBanJson))
            {
                _logger.LogError("DatBanViewModel not found in Redis for TempTransactionId: {Id}", tempTransactionId);
                return (false, null);
            }

            var model = JsonSerializer.Deserialize<DatBanViewModel>(datBanJson);

            var datBan = new DatBan
            {
                KhachHangId = 0,
                ThoiGianDatBan = model.ThoiGianDatBan,
                SoLuongNguoi = model.SoLuongNguoi,
                CocTien = model.CocTien,
                Loai = model.Loai,
                ThoiGianTao = DateTime.Now,
                IsDatHo = model.IsDatHo,
                TenLienHe = model.TenLienHe,
                SDTLienHe = model.SDTLienHe,
                GhiChu = model.GhiChu,
                TrangThai = TrangThaiBanDat.ChoXacNhan
            };

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.SDT == model.SDT);
            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    TenKhachHang = model.TenKhachHang ?? "Khách vãng lai",
                    SDT = model.SDT
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Created new KhachHang with ID: {Id}", khachHang.Id);
            }
            datBan.KhachHangId = khachHang.Id;

            _context.DatBans.Add(datBan);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved DatBan with ID: {Id}", datBan.Id);

            await _cache.RemoveAsync($"TempDatBan:{tempTransactionId}");

            return (true, datBan);
        }

        private string HmacSha512(string input, string key)
        {
            var hash = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}