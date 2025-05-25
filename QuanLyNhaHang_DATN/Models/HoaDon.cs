using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_DATN.Models
{
    public class HoaDon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DatBanId { get; set; }
        public int? NhanVienId { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayThanhToan { get; set; }

        public decimal TongTienGoiMon { get; set; }
        public decimal TongTienThanhToan { get; set; }
        public decimal? GiamGia { get; set; }

        public TrangThaiHoaDon TrangThai { get; set; }

        public PhuongThucThanhToan? PhuongThuc { get; set; }

        [ForeignKey("DatBanId")]
        public DatBan DatBan { get; set; }

       
        [ForeignKey("NhanVienId")]
        [ValidateNever]
        public NhanVien NhanVien { get; set; }
    }
     public enum TrangThaiHoaDon
     {
        [Display(Name = "Đã thanh toán")]
        DaThanhToan = 1,
        [Display(Name = "Chưa thanh toán")]
        ChuaThanhToan =0
     }
    public enum PhuongThucThanhToan
    {
        [Display(Name = "Tiền mặt")]
        TienMat = 1,
        [Display(Name = "Chuyển khoản")]
        ChuyenKhoan =2,
        [Display(Name = "Thẻ tín dụng")]
        TheTinDung =3

    }
}
