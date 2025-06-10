using System.ComponentModel.DataAnnotations;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class DatBanViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string TenKhachHang { get; set; }
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        public string SDT { get; set; }

        public bool IsDatHo { get; set; }

       
        public string? TenLienHe { get; set; }

        //[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số")]
        public string? SDTLienHe { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian đặt bàn")]
        public DateTime ThoiGianDatBan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng người")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng người phải lớn hơn 0")]
        public int SoLuongNguoi { get; set; }
        [Required(ErrorMessage = "Số tiền cọc không hợp lệ")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền cọc không hợp lệ")]
        public decimal CocTien { get; set; }
        public string? GhiChu { get; set; }
        public LoaiDatBan Loai { get; set; }
        public TrangThaiBanDat TrangThai { get; set; }
        public string? TrangThaiDisplay { get; set; }
        public string? LyDoHuy { get; set; }
        //public int? BanId { get; set; }
        public string? Bans { get; set; }
        public string? TenNhanVien { get; set; }
    }
}