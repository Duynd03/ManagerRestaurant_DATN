using Microsoft.AspNetCore.Identity;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Services.TaiKhoanService;

namespace QuanLyNhaHang_DATN.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var taiKhoanService = scope.ServiceProvider.GetRequiredService<ITaiKhoanService>();

            // Đảm bảo cơ sở dữ liệu đã được tạo
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();

            // Tạo tài khoản Admin mặc định (QuyenId = 1)
            var adminResult = await taiKhoanService.CreateUserAsync(
                username: "admin",
                password: "Admin@123",
                quyenId: 1, // Admin
                tenNhanVien: null, // Admin không cần thông tin nhân viên
                sdt: null,
                ngaySinh: null,
                diaChi: null
            );
            if (!adminResult.Success)
            {
                Console.WriteLine($"Lỗi khi tạo tài khoản admin: {adminResult.Message}");
            }

            // Tạo tài khoản Nhân viên phục vụ 1 (QuyenId = 2)
            var nhanVien1Result = await taiKhoanService.CreateUserAsync(
                username: "ndduy",
                password: "Duy@123",
                quyenId: 2, // Nhân viên phục vụ
                tenNhanVien: "Nguyễn Đình Duy",
                sdt: "0123456789",
                ngaySinh: new DateTime(2003, 1, 1),
                diaChi: "Hà Nội"
            );
            if (!nhanVien1Result.Success)
            {
                Console.WriteLine($"Lỗi khi tạo tài khoản : {nhanVien1Result.Message}");
            }

            // Tạo tài khoản Nhân viên phục vụ 2 (QuyenId = 2)
            var nhanVien2Result = await taiKhoanService.CreateUserAsync(
                username: "nttam",
                password: "Tam@123",
                quyenId: 2, // Nhân viên phục vụ
                tenNhanVien: "Nguyễn Thị Tâm",
                sdt: "0123456790",
                ngaySinh: new DateTime(1996, 2, 2),
                diaChi: "Hà Nội"
            );
            if (!nhanVien2Result.Success)
            {
                Console.WriteLine($"Lỗi khi tạo tài khoản : {nhanVien2Result.Message}");
            }

            // Tạo tài khoản Kế toán 1 (QuyenId = 3)
            var keToan1Result = await taiKhoanService.CreateUserAsync(
                username: "nhdang",
                password: "Dang@123",
                quyenId: 3, // Kế toán
                tenNhanVien: "Nguyễn Hải Đăng",
                sdt: "0123456791",
                ngaySinh: new DateTime(1990, 3, 3),
                diaChi: "Hà Nội"
            );
            if (!keToan1Result.Success)
            {
                Console.WriteLine($"Lỗi khi tạo tài khoản : {keToan1Result.Message}");
            }

            // Tạo tài khoản Kế toán 2 (QuyenId = 3)
            var keToan2Result = await taiKhoanService.CreateUserAsync(
                username: "ntndiep",
                password: "Diep@123",
                quyenId: 3, // Kế toán
                tenNhanVien: "Nguyễn Thị Ngọc Diệp",
                sdt: "0123456792",
                ngaySinh: new DateTime(1991, 4, 4),
                diaChi: "Hà Nội"
            );
            if (!keToan2Result.Success)
            {
                Console.WriteLine($"Lỗi khi tạo tài khoản : {keToan2Result.Message}");
            }
        }
    }
}