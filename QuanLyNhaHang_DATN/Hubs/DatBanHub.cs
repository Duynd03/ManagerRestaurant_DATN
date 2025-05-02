using Microsoft.AspNetCore.SignalR;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Hubs
{
    public class DatBanHub : Hub
    {
        // Thêm client (admin) vào nhóm "Admins" khi kết nối.
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
        //Gửi thông tin đặt bàn (DatBan) đến nhóm "Admins" khi có đặt bàn mới.
        public async Task SendDatBanUpdate(DatBan datBan)
        {
            await Clients.Group("Admins").SendAsync("ReceiveDatBanUpdate", datBan);
        }
        //Gửi thông báo xóa đặt bàn (dựa trên datBanId) khỏi danh sách chờ khi admin xếp bàn xong.
        public async Task SendRemoveDatBan(int datBanId)
        {
            await Clients.Group("Admins").SendAsync("RemoveDatBan", datBanId);
        }
    }
}