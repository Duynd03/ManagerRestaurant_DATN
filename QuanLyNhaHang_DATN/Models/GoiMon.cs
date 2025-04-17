using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHang_DATN.Models
{
    public class GoiMon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MonAnId { get; set; }

        [Required]
        public int DatBanId { get; set; }

        public int SoLuong { get; set; }

        public decimal Gia { get; set; }

        [StringLength(100)]
        public string GhiChu { get; set; }

        public DateTime? ThoiGianGoiMon { get; set; }

        [ForeignKey("MonAnId")]
        public MonAn MonAn { get; set; }

        [ForeignKey("DatBanId")]
        public DatBan DatBan { get; set; }
    }
}
