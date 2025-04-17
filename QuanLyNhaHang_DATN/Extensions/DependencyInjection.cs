using QuanLyNhaHang_DATN.Repositories.BanRepository;
using QuanLyNhaHang_DATN.Repositories.DanhMucRepository;
using QuanLyNhaHang_DATN.Repositories.KhuVucBanRepository;
using QuanLyNhaHang_DATN.Repositories.MonAnRepository;
using QuanLyNhaHang_DATN.Services.BanService;
using QuanLyNhaHang_DATN.Services.DanhMucService;
using QuanLyNhaHang_DATN.Services.KhuVucBanService;
using QuanLyNhaHang_DATN.Services.MonAnService;

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
            return services;
          
        }
    }
}
