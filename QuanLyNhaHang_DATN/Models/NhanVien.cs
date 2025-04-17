using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.Models
{
    public class NhanVien
    {
        [Key]
        public int Id { get; set; }

        [Required]

        public string TenNhanVien { get; set; }

        [Required]
        public string Sdt { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string? DiaChi { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int? TaiKhoanId { get; set; }

        [ForeignKey("TaiKhoanId")]
        public TaiKhoan TaiKhoan { get; set; }
    }
}
