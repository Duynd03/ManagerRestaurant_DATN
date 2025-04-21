using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories;
using QuanLyNhaHang_DATN.ViewModels;

namespace QuanLyNhaHang_DATN.Services.TaiKhoanService
{
    public interface ITaiKhoanService : IBaseService<TaiKhoan>
    {
        Task<Result<TaiKhoan>> DangKyAsync(DangKyViewModel model);
        Task<Result<TaiKhoan>> DangNhapAsync(DangNhapViewModel model);
    }
}
