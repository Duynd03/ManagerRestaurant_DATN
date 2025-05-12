using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyNhaHang_DATN.Models
{
    public class MonAn
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên món ăn là bắt buộc")]
        public string TenMonAn { get; set; }
        [Required(ErrorMessage = "Giá món ăn là bắt buộc")]
        public decimal Gia { get; set; }
        public string? MoTa { get; set; }

        //[Required(ErrorMessage = "Hình ảnh là bắt buộc")]
        public string? HinhAnh { get; set; }

        [Required(ErrorMessage = "Trạng thái món ăn là bắt buộc")]
        public TrangThaiMonAn TrangThai { get; set; }

        [Required(ErrorMessage = "Danh mục món ăn là bắt buộc")]
        public int DanhMucId { get; set; }

        [ForeignKey("DanhMucId")]
        [ValidateNever]
        public DanhMuc DanhMuc { get; set; }
        [ValidateNever]
        public virtual ICollection<GoiMon> GoiMons { get; set; }
    }
    public enum TrangThaiMonAn
    {
        [Display(Name = "Có sẵn")]
        CoSan = 1,
        [Display(Name = "Hết hàng")]
        HetHang = 0
    }
}
