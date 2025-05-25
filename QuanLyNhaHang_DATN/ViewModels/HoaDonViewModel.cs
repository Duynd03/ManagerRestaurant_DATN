namespace QuanLyNhaHang_DATN.ViewModels
{
    public class HoaDonViewModel
    {
        public int Id { get; set; }
        public string MaHoaDon { get; set; }
        public int DatBanId { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public string? TenLienHe { get; set; }
        public string? SDTLienHe { get; set; }
        public DateTime ThoiGianDatBan { get; set; }
        public DateTime ThoiGianThanhToan { get; set; }
        public decimal TongTienGoiMon { get; set; }
        public decimal TienCoc { get; set; } 
        public decimal ThueVat { get; set; } 
        public decimal TongTienThanhToan { get; set; } 
        public decimal? GiamGia { get; set; }
        public string PhuongThucThanhToanDisplay { get; set; }
        public string TrangThaiDisplay { get; set; }
        public string Bans {  get; set; }
        public string? TenNhanVien { get; set; }
    }
}