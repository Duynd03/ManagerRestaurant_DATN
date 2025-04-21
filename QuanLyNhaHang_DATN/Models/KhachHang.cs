using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyNhaHang_DATN.Models
{
    public class KhachHang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TenKhachHang { get; set; }

        [Required]
        public string SDT { get; set; }

        public string? Email { get; set; }

        public string? DiaChi { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public int? TaiKhoanId { get; set; }

        [ForeignKey("TaiKhoanId")]
        [ValidateNever]
        public TaiKhoan TaiKhoan { get; set; }
    }
}
