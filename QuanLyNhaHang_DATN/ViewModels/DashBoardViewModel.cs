using System;
using System.Collections.Generic;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class DashBoardViewModel
    {
        public int SoHoaDonDaThanhToan { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoLuongKhachHang { get; set; }
        public List<DoanhThuTheoThoiGian> DoanhThuTheoThoiGian { get; set; }
        public List<DoanhThuTheoThoiGian> DoanhThuSoSanh { get; set; }
        public bool ShowChart { get; set; }
        public string ChartTitle { get; set; }
        public string CompareTitle { get; set; }
    }

    public class DoanhThuTheoThoiGian
    {
        public string Label { get; set; }
        public decimal DoanhThu { get; set; }
    }
}