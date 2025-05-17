namespace QuanLyNhaHang_DATN.ViewModels
{
    public class TrangThaiBanViewModel
    {
        public int TrangThaiValue { get; set; } // Giá trị enum trạng thái bàn (Trong, DaDatTruoc, DangSuDung)
        public string TrangThaiDisplay { get; set; } // Tên hiển thị trạng thái (ví dụ: "Trống", "Đã đặt trước")
    }
}
