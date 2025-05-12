using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyNhaHang_DATN.Models
{
    public class Ban
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(30)]
        [Required(ErrorMessage = "Tên bàn là bắt buộc")]
        public string TenBan { get; set; }
        [Required(ErrorMessage = "Khu vực bàn là bắt buộc")]
        public int KhuVucBanId { get; set; }
        [Required(ErrorMessage = "Trạng thái bàn là bắt buộc")]
        public TrangThaiBan TrangThai { get; set; } 

        [ForeignKey("KhuVucBanId")]
        [ValidateNever]
        public KhuVucBan KhuVucBan { get; set; }
        [ValidateNever]
        public virtual ICollection<DatBan_Ban> DatBanBans { get; set; }

        //[ValidateNever]
        //public virtual ICollection<DatBan> DatBans { get; set; }
    }
    public enum TrangThaiBan
    {
        [Display(Name = "Trống")]
        Trong = 0,
        [Display(Name = "Đã đặt trước")]
        DaDatTruoc = 1,
        [Display(Name = "Đang sử dụng")]
        DangSuDung = 2,
        [Display(Name = "Đã hủy")]
        Huy = 3
    }
}
