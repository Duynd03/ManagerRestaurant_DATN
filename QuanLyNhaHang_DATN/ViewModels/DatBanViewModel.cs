using System.ComponentModel.DataAnnotations;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class DatBanViewModel
    {
        public int Id { get; set; }

        public string TenKhachHang { get; set; }
        public string SDT { get; set; }

        // Trường để xác định có phải đặt hộ hay không
        public bool IsDatHo { get; set; }

        // Chỉ bắt buộc khi IsDatHo = true 
        //[Required(ErrorMessage = "Vui lòng nhập họ và tên")]
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

        public decimal CocTien { get; set; }
        public string? GhiChu { get; set; }
        public LoaiDatBan Loai { get; set; }
        public TrangThaiBanDat TrangThai { get; set; }
        public string? TrangThaiDisplay { get; set; }
        public int? BanId { get; set; }
    }
}