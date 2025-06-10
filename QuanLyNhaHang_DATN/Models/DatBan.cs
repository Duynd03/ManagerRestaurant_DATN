using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_DATN.Models
{
    public class DatBan
    {
        [Key]
        public int Id { get; set; }
        public int? KhachHangId { get; set; }

        public int? NhanVienId { get; set; }
       
        public DateTime ThoiGianDatBan { get; set; }
        public int SoLuongNguoi { get; set; }
        public decimal CocTien { get; set; }
        public string? GhiChu { get; set; }
        public bool? IsDatHo { get; set; }
        public string? TenLienHe { get; set; }
        public string? SDTLienHe { get; set; }

        //public int? BanId { get; set; }
        public DateTime ThoiGianTao { get; set; } = DateTime.Now;
        public DateTime? ThoiGianKetThuc { get; set; }
        public TrangThaiBanDat TrangThai { get; set; }
        public LoaiDatBan Loai { get; set; }
        public string? LyDoHuy { get; set; }

        [ForeignKey("KhachHangId")]
        [ValidateNever]
        public KhachHang KhachHang { get; set; }

        [ForeignKey("NhanVienId")]
        [ValidateNever]
        public NhanVien NhanVien { get; set; }

        //[ForeignKey("BanId")]
        //[ValidateNever]
        //public Ban Ban { get; set; }
        [ValidateNever]
        public virtual ICollection<DatBan_Ban> DatBanBans { get; set; }

        [ValidateNever]
        public virtual ICollection<GoiMon> GoiMons { get; set; }

    }
    public enum LoaiDatBan
    {
        [Display(Name = "Đặt trước")]
        DatTruoc = 1,
        [Display(Name = "Đặt khi đến")]
        DatKhiDen = 0

    }
    public enum TrangThaiBanDat
    {
        [Display(Name = "Chờ xác nhận")]
        ChoXacNhan =0,
        [Display(Name = "Đã xác nhận")]
        DaXacNhan =1,
        [Display(Name = "Đã Hủy")]
        DaHuy =2,
        [Display(Name = "Hoàn thành")]
         HoanThanh =3
    }
}
