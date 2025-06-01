using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.ViewModels
{
    public class MenuViewModel
    {
        public IEnumerable<DanhMuc> DanhMucList { get; set; }
        public IEnumerable<MonAn> MonAnList { get; set; }
    }
}
