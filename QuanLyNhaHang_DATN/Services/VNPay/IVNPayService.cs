using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.ViewModels;

namespace QuanLyNhaHang_DATN.Services.VNPay
{
    public interface IVNPayService
    {
        Task<string> CreatePaymentUrl(DatBanViewModel model, HttpContext httpContext);
        Task<(bool Success, DatBan DatBan)> ProcessCallback(IQueryCollection query, string ipnType);
    }
}
