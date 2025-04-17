using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyNhaHang_DATN.Models
{
    public class TaiKhoan
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TenTaiKhoan { get; set; }
        [Required]
        public string MatKhau { get; set; }
       
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public TrangThaiTaiKhoan TrangThai { get; set; }
        public int QuyenId { get; set; }

        [ForeignKey("QuyenId")]
        public Quyen Quyen { get; set; }
        public ICollection<HoaDon> HoaDons { get; set; }
    }
    public enum TrangThaiTaiKhoan
    {
        [Display(Name = "Đang hoạt động")]
        DangHoatDong = 1,
        [Display(Name = "Đã khóa")]
        DaKhoa = 0
    }
}
