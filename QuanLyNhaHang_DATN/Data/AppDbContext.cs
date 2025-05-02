using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Data
{
    public class AppDbContext : IdentityDbContext<TaiKhoan, Quyen, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<MonAn> MonAns { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<KhuVucBan> KhuVucBans { get; set; }
        public DbSet<DatBan> DatBans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<Quyen> Quyens { get; set; }
        public DbSet<ChucNang> ChucNangs { get; set; }
        public DbSet<Quyen_ChucNang> Quyen_ChucNangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<GoiMon> GoiMons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ánh xạ các bảng Identity
            builder.Entity<TaiKhoan>().ToTable("TaiKhoan");
            builder.Entity<Quyen>().ToTable("Quyen");

            // Ánh xạ các bảng khác
            builder.Entity<MonAn>().ToTable("MonAn");
            builder.Entity<DanhMuc>().ToTable("DanhMuc");
            builder.Entity<Ban>().ToTable("Ban");
            builder.Entity<KhuVucBan>().ToTable("KhuVucBan");
            builder.Entity<DatBan>().ToTable("DatBan");
            builder.Entity<KhachHang>().ToTable("KhachHang");
            builder.Entity<NhanVien>().ToTable("NhanVien");
            builder.Entity<ChucNang>().ToTable("ChucNang");
            builder.Entity<Quyen_ChucNang>().ToTable("Quyen_ChucNang");
            builder.Entity<HoaDon>().ToTable("HoaDon");
            builder.Entity<GoiMon>().ToTable("GoiMon");
        }
    }
}