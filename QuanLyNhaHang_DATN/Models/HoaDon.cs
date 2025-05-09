﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_DATN.Models
{
    public class HoaDon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DatBanId { get; set; }

        public int? TaiKhoanId { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        public decimal TongTien { get; set; }

        public decimal? GiamGia { get; set; }

        public TrangThaiHoaDon TrangThai { get; set; }

        public PhuongThucThanhToans PhuongThucThanhToan { get; set; }

        [ForeignKey("DatBanId")]
        public DatBan DatBan { get; set; }

        [ForeignKey("TaiKhoanId")]
        public TaiKhoan TaiKhoan { get; set; }
    }
     public enum TrangThaiHoaDon
     {
        [Display(Name = "Đã thanh toán")]
        DaThanhToan = 1,
        [Display(Name = "Chưa thanh toán")]
        ChuaThanhToan =0
     }
    public enum PhuongThucThanhToans
    {
        [Display(Name = "Tiền mặt")]
        TienMat = 1,
        [Display(Name = "Chuyển khoản")]
        ChuyenKhoan =2,
        [Display(Name = "Thẻ tín dụng")]
        TheTinDung =3

    }
}
