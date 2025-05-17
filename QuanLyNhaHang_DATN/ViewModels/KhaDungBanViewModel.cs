namespace QuanLyNhaHang_DATN.ViewModels
{
    public class KhaDungBanViewModel
    {
        public bool KhaDung { get; set; } // Bàn có sẵn cho đặt mới không
        public List<LichDatViewModel> LichDat { get; set; } = new List<LichDatViewModel>(); // Danh sách lịch đặt
        public TrangThaiBanViewModel TrangThai { get; set; } // Trạng thái hiện tại của bàn
        public bool IsCurrentTable { get; set; } // Bàn có phải là bàn hiện tại của đơn đặt bàn không
    }
    public class LichDatViewModel
    {
        public int DatBanId { get; set; } // ID của đơn đặt bàn
        public DateTime ThoiGianBatDau { get; set; } // Thời gian bắt đầu đặt
        public string ThoiGianBatDauFormatted => ThoiGianBatDau.ToString("HH:mm dd/MM/yyyy"); // Định dạng thời gian cho giao diện

    }
}
