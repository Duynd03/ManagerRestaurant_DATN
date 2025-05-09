namespace QuanLyNhaHang_DATN.Models
{
    public class DatBan_Ban
    {
        public int DatBanId { get; set; }
        public DatBan DatBan { get; set; }
        public int BanId { get; set; }
        public Ban Ban { get; set; }
    }
}
