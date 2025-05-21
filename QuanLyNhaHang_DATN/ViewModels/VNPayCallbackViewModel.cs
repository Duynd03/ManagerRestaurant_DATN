namespace QuanLyNhaHang_DATN.ViewModels
{
    public class VNPayCallbackViewModel
    {
        public string VNPayTransactionId { get; set; }
        public string TempTransactionId { get; set; }
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; }
        public string SecureHash { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
