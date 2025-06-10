using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Extensions;
using Microsoft.AspNetCore.Identity;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Hubs;
using System.Net;
using QuanLyNhaHang_DATN.Config;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Rotativa.AspNetCore;
using DinkToPdf.Contracts;
using DinkToPdf;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm Identity
builder.Services.AddIdentity<TaiKhoan, Quyen>(options =>
{
    // Cấu hình mật khẩu
    options.Password.RequireDigit = true; // Bắt buộc có số
    options.Password.RequiredLength = 6; // độ dài tối thiểu
    options.Password.RequireNonAlphanumeric = false; // ký tự đặc biệt
    options.Password.RequireUppercase = false; //ký tự in hoa
    options.Password.RequireLowercase = false; //  ký tự in thường

    // Cấu hình khóa tài khoản
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình đăng nhập
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Cấu hình cookie xác thực
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Account/DangNhap"; // Đường dẫn đăng nhập cho Admin
    options.AccessDeniedPath = "/Admin/Account/AccessDenied"; // Đường dẫn khi bị từ chối quyền
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// Thêm Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminArea", policy =>
        policy.RequireRole("Admin", "NhanVien", "KeToan"));
});

// Thêm dịch vụ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout sau 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Đảm bảo HTTPS
});

// Thêm SignalR
builder.Services.AddSignalR();

// Thêm HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Thêm cấu hình VNPayConfig
builder.Services.Configure<VNPayConfig>(builder.Configuration.GetSection("VNPayConfig"));

// Cấu hình PDF
builder.Services.AddSingleton<IConverter>(sp =>
{
    var context = sp.GetService<IWebHostEnvironment>();
    var path = context?.ContentRootPath ?? Directory.GetCurrentDirectory();
    var wkHtmlToPdfDir = Path.Combine(path, "wwwroot", "lib", "wkhtmltopdf");

    // Đặt biến môi trường PATH trước khi khởi tạo PdfTools
    var currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
    Environment.SetEnvironmentVariable("PATH", wkHtmlToPdfDir + Path.PathSeparator + currentPath);

    // Tạo thư mục tạm
    var tempFolder = Path.Combine(path, "wwwroot", "temp");
    if (!Directory.Exists(tempFolder))
    {
        Directory.CreateDirectory(tempFolder);
    }

    // Khởi tạo PdfTools mà không cần thuộc tính bổ sung
    var pdfTools = new PdfTools();

    return new SynchronizedConverter(pdfTools);
});


//
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NhaHangConnection")));

builder.Services.AddRepositoriesAndServices();
var app = builder.Build();

// Khởi tạo dữ liệu (vai trò, tài khoản mặc định, v.v.)
await DataSeeder.SeedDataAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();
app.UseSession();
//app.MapControllerRoute(
//    name: "Areas",
//    pattern: "{area:exists}/{controller=DashBoard}/{action=Index}/{id?}"
//);
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints => // định nghĩa tất cả các endpoint
{
    endpoints.MapHub<DatBanHub>("/datBanHub"); //ánh xạ endpoint cho SignalR Hub.
    endpoints.MapControllerRoute(
        name: "Areas",
        pattern: "{area:exists}/{controller=DashBoard}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
