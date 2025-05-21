namespace QuanLyNhaHang_DATN.ViewModels
{
    public class VNPayRequestViewModel
    {
        public string TempTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
