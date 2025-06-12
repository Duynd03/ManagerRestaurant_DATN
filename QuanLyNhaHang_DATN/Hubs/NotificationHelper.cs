using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.ViewModels;
using System.Text.Json;

namespace QuanLyNhaHang_DATN.Hubs
{
    public static class NotificationHelper
    {
        public static async Task SendDatBanNotificationAsync(
            IHubContext<DatBanHub> hubContext,
            IDistributedCache cache,
            string loaiThongBao,
            DatBan datBan)
        {
            string message = loaiThongBao switch
            {
                "NEW" => $"Bạn có đơn đặt bàn mới ID {datBan.Id} lúc {datBan.ThoiGianDatBan:HH:mm dd/MM/yyyy}",
                "XEPBAN" => $"Đã xếp bàn cho đơn đặt bàn ID {datBan.Id} lúc {DateTime.Now:HH:mm dd/MM/yyyy}",
                "CHUYENBAN" => $"Đã chuyển bàn cho đơn đặt bàn ID {datBan.Id} lúc {DateTime.Now:HH:mm dd/MM/yyyy}",
                "HUY" => $"Đơn đặt bàn ID {datBan.Id} đã bị hủy lúc {DateTime.Now:HH:mm dd/MM/yyyy}",
                _ => $"Cập nhật đơn đặt bàn ID {datBan.Id}"
            };

            var notification = new NotificationModel
            {
                DatBanId = datBan.Id,
                Message = message,
                Time = DateTime.Now,
                LoaiThongBao = loaiThongBao
            };

            var key = "notifs:all";
            var existingNotifsJson = await cache.GetStringAsync(key) ?? "[]";
            var existingNotifs = JsonSerializer.Deserialize<List<NotificationModel>>(existingNotifsJson) ?? new List<NotificationModel>();
            existingNotifs.Insert(0, notification);
            if (existingNotifs.Count > 50) existingNotifs.RemoveRange(50, existingNotifs.Count - 50);

            await cache.SetStringAsync(key, JsonSerializer.Serialize(existingNotifs), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });

            await hubContext.Clients.Group("Admins")
                .SendAsync("ReceiveBookingNotification", notification, loaiThongBao);
        }
    }
}
