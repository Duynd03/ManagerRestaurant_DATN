using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_DATN.Models
{
    public class BanSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BanId { get; set; }

        [ForeignKey("BanId")]
        public Ban Ban { get; set; }

        [Required]
        public int DatBanId { get; set; }

        [ForeignKey("DatBanId")]
        public DatBan DatBan { get; set; }

        [Required]
        public DateTime ThoiGianBatDau { get; set; }

        public DateTime? ThoiGianKetThuc { get; set; }

        [Required]
        public TrangThaiBan TrangThai { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }
    }
}