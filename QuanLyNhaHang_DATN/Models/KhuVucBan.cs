using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHang_DATN.Models
{
    public class KhuVucBan
    {
        [Key]
        public int Id { get; set; }
        public string TenKhuVuc { get; set; }
        public string? MoTa { get; set; }
        public int? SoLuongBan { get; set; }
        [ValidateNever]
        public virtual ICollection<Ban> Bans { get; set; }
    }
}
