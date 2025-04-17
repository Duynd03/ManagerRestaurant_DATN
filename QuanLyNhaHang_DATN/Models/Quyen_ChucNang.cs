using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.Models
{
    public class Quyen_ChucNang
    {
        [Key]
        public int Id { get; set; }

        public int QuyenId { get; set; }

        public int ChucNangId { get; set; }

        [ForeignKey("QuyenId")]
        public Quyen Quyen { get; set; }

        [ForeignKey("ChucNangId")]
        public ChucNang ChucNang { get; set; }
    }
}
