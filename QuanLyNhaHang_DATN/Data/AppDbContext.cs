using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
