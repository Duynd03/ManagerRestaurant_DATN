using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.Models
{
    public class Quyen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TenQuyen { get; set; }

        public ICollection<Quyen_ChucNang> Quyen_ChucNangs { get; set; }

    }
}
