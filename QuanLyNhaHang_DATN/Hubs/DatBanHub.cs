using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Hubs
{
    //[Authorize]
    public class DatBanHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[SignalR] User {Context.User?.Identity?.Name} connected with ConnectionId: {Context.ConnectionId} at {DateTime.Now}");
            if (Context.User?.Identity?.IsAuthenticated == true &&
                (Context.User.IsInRole("Admin") || Context.User.IsInRole("NhanVien")))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                Console.WriteLine($"[SignalR] {Context.ConnectionId} joined Admins group at {DateTime.Now}");
            }
            else
            {
                Console.WriteLine($"[SignalR] User {Context.User?.Identity?.Name} not authorized for Admins group at {DateTime.Now}");
            }
            await base.OnConnectedAsync();
        }

        //public async Task SendBookingNotification(DatBan datBan, string loaiThongBao)
        //{
        //    Console.WriteLine($"[SignalR] Sending to Admins group for DatBanId {datBan.Id} at {DateTime.Now}");
        //    var notification = new
        //    {
        //        DatBanId = datBan.Id,
        //        TenKhachHang = datBan.KhachHang?.TenKhachHang ?? "Khách vãng lai",
        //        ThoiGianDatBan = datBan.ThoiGianDatBan.ToString("HH:mm dd/MM/yyyy"),
        //        LoaiThongBao = loaiThongBao
        //    };
        //    await Clients.Group("Admins").SendAsync("ReceiveBookingNotification", notification, loaiThongBao);
        //    Console.WriteLine($"[SignalR] Sent to Admins group for DatBanId {datBan.Id} at {DateTime.Now}");
        //}


        //// Thêm client (admin) vào nhóm "Admins" khi kết nối.
        //public async Task JoinAdminGroup()
        //{
        //    var user = Context.User;
        //    if (user.IsInRole("Admin") || user.IsInRole("NhanVien"))
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        //    }
        //}
        ////Gửi thông tin đặt bàn (DatBan) đến nhóm "Admins" khi có đặt bàn mới.
        //public async Task SendDatBanUpdate(DatBan datBan)
        //{
        //    await Clients.Group("Admins").SendAsync("ReceiveDatBanUpdate", datBan);
        //}
        ////Gửi thông báo xóa đặt bàn (dựa trên datBanId) khỏi danh sách chờ khi admin xếp bàn xong.
        //public async Task SendRemoveDatBan(int datBanId)
        //{
        //    await Clients.Group("Admins").SendAsync("RemoveDatBan", datBanId);
        //}
    }
}