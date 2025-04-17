using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.Models
{
    public class DanhMuc
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string TenDanhMuc { get; set; }
        public string?  MoTa {  get; set; }
        [ValidateNever]
        public virtual ICollection<MonAn> MonAns { get; set; }
    }
}
