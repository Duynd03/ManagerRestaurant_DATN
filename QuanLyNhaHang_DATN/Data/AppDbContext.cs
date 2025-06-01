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
       
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<GoiMon> GoiMons { get; set; }
        public DbSet<DatBan_Ban> DatBan_Bans { get; set; }
        public DbSet<BanSchedule> BanSchedules { get; set; }

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
          
            builder.Entity<HoaDon>().ToTable("HoaDon");
            builder.Entity<GoiMon>().ToTable("GoiMon");
            builder.Entity<DatBan_Ban>().ToTable("DatBan_Ban");
            builder.Entity<BanSchedule>().ToTable("BanSchedule");
            // Cấu hình khóa chính composite cho DatBan_Ban
            builder.Entity<DatBan_Ban>()
                .HasKey(db => new { db.DatBanId, db.BanId });

            // Cấu hình quan hệ nhiều-nhiều cho DatBan_Ban
            builder.Entity<DatBan_Ban>()
                .HasOne(db => db.DatBan)
                .WithMany(d => d.DatBanBans)
                .HasForeignKey(db => db.DatBanId);

            builder.Entity<DatBan_Ban>()
                .HasOne(db => db.Ban)
                .WithMany(b => b.DatBanBans)
                .HasForeignKey(db => db.BanId);

            // Cấu hình khóa chính composite cho Quyen_ChucNang (nếu cần)
            //builder.Entity<Quyen_ChucNang>()
            //    .HasKey(qc => new { qc.IdQuyen, qc.IdChucNang });
        }
    }
}