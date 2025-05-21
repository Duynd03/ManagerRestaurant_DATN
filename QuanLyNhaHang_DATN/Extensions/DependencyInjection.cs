using QuanLyNhaHang_DATN.Repositories.BanRepository;
using QuanLyNhaHang_DATN.Repositories.DanhMucRepository;
using QuanLyNhaHang_DATN.Repositories.DatBanRepository;
using QuanLyNhaHang_DATN.Repositories.GoiMonRepository;
using QuanLyNhaHang_DATN.Repositories.HoaDonRepository;
using QuanLyNhaHang_DATN.Repositories.KhachHangRepository;
using QuanLyNhaHang_DATN.Repositories.KhuVucBanRepository;
using QuanLyNhaHang_DATN.Repositories.MonAnRepository;
using QuanLyNhaHang_DATN.Repositories.NhanVienRepository;
using QuanLyNhaHang_DATN.Repositories.TaiKhoanRepository;
using QuanLyNhaHang_DATN.Services.BanService;
using QuanLyNhaHang_DATN.Services.DanhMucService;
using QuanLyNhaHang_DATN.Services.DashBoardService;
using QuanLyNhaHang_DATN.Services.DatBanService;
using QuanLyNhaHang_DATN.Services.GoiMonService;
using QuanLyNhaHang_DATN.Services.HoaDonService;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using QuanLyNhaHang_DATN.Services.KhuVucBanService;
using QuanLyNhaHang_DATN.Services.MonAnService;
using QuanLyNhaHang_DATN.Services.NhanVienService;
using QuanLyNhaHang_DATN.Services.TaiKhoanService;
using QuanLyNhaHang_DATN.Services.VNPay;

namespace QuanLyNhaHang_DATN.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services)
        {
            // DanhMuc
            services.AddScoped<IDanhMucRepository, DanhMucRepository>();

            services.AddScoped<IDanhMucService, DanhMucService>();
            //Mon an
            services.AddScoped<IMonAnRepository, MonAnRepository>();

            services.AddScoped<IMonAnService, MonAnService>();
            //Khu Vực Bàn
            services.AddScoped<IKhuVucBanRepository, KhuVucBanRepository>();

            services.AddScoped<IKhuVucBanService, KhuVucBanService>();
            //Ban
            services.AddScoped<IBanRepository, BanRepository>();

            services.AddScoped<IBanService, BanService>();

            //Tai khoan
            services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();

            services.AddScoped<ITaiKhoanService, TaiKhoanService>();
            //Khach Hang
            services.AddScoped<IKhachHangRepository, KhachHangRepository>();

            services.AddScoped<IKhachHangService, KhachHangService>();
            //DatBan 
            services.AddScoped<IDatBanRepository, DatBanRepository>();

            services.AddScoped<IDatBanService, DatBanService>();
            // Nhan Vien
            services.AddScoped<INhanVienRepository, NhanVienRepository>();

            services.AddScoped<INhanVienService, NhanVienService>();
            // Goi mon
            services.AddScoped<IGoiMonRepository, GoiMonRepository>();

            services.AddScoped<IGoiMonService, GoiMonService>();

            //Hoa Don
            services.AddScoped<IHoaDonRepository, HoaDonRepository>();
            services.AddScoped<IHoaDonService, HoaDonService>();

            //Dashboard
            services.AddScoped<IDashBoardService, DashBoardService>();
            // VNPay
            services.AddScoped<IVNPayService, VNPayService>();
            return services;
            
        }
    }
}
