﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuanLyNhaHang_DATN.Data;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250418033320_UpdateQuyen")]
    partial class UpdateQuyen
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.Ban", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("KhuVucBanId")
                        .HasColumnType("int");

                    b.Property<string>("TenBan")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("TrangThai")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KhuVucBanId");

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.ChucNang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("MoTa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenChucNang")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ChucNangs");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.DanhMuc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("MoTa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenDanhMuc")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DanhMucs");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.DatBan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BanId")
                        .HasColumnType("int");

                    b.Property<decimal>("CocTien")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("GhiChu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("KhachHangId")
                        .HasColumnType("int");

                    b.Property<int>("Loai")
                        .HasColumnType("int");

                    b.Property<int?>("NhanVienId")
                        .HasColumnType("int");

                    b.Property<int>("SoLuongNguoi")
                        .HasColumnType("int");

                    b.Property<DateTime>("ThoiGianDatBan")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ThoiGianKetThuc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ThoiGianTao")
                        .HasColumnType("datetime2");

                    b.Property<int>("TrangThai")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BanId");

                    b.HasIndex("KhachHangId");

                    b.HasIndex("NhanVienId");

                    b.ToTable("DatBans");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.GoiMon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DatBanId")
                        .HasColumnType("int");

                    b.Property<string>("GhiChu")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Gia")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MonAnId")
                        .HasColumnType("int");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ThoiGianGoiMon")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DatBanId");

                    b.HasIndex("MonAnId");

                    b.ToTable("GoiMons");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.HoaDon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DatBanId")
                        .HasColumnType("int");

                    b.Property<decimal?>("GiamGia")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("MaHoaDon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgayThanhToan")
                        .HasColumnType("datetime2");

                    b.Property<int>("PhuongThucThanhToan")
                        .HasColumnType("int");

                    b.Property<int?>("TaiKhoanId")
                        .HasColumnType("int");

                    b.Property<decimal>("TongTien")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TrangThai")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DatBanId");

                    b.HasIndex("TaiKhoanId");

                    b.ToTable("HoaDons");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.KhachHang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DiaChi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("SDT")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TaiKhoanId")
                        .HasColumnType("int");

                    b.Property<string>("TenKhachHang")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TaiKhoanId");

                    b.ToTable("KhachHangs");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.KhuVucBan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("MoTa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SoLuongBan")
                        .HasColumnType("int");

                    b.Property<string>("TenKhuVuc")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("KhuVucBans");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.MonAn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DanhMucId")
                        .HasColumnType("int");

                    b.Property<double>("Gia")
                        .HasColumnType("float");

                    b.Property<string>("HinhAnh")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MoTa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenMonAn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TrangThai")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DanhMucId");

                    b.ToTable("MonAns");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.NhanVien", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DiaChi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("NgaySinh")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<string>("Sdt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TaiKhoanId")
                        .HasColumnType("int");

                    b.Property<string>("TenNhanVien")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TaiKhoanId");

                    b.ToTable("NhanViens");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.Quyen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("MoTa")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenQuyen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Quyens");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.Quyen_ChucNang", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ChucNangId")
                        .HasColumnType("int");

                    b.Property<int>("QuyenId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChucNangId");

                    b.HasIndex("QuyenId");

                    b.ToTable("Quyen_ChucNangs");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.TaiKhoan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("MatKhau")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<int>("QuyenId")
                        .HasColumnType("int");

                    b.Property<string>("TenTaiKhoan")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TrangThai")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuyenId");

                    b.ToTable("TaiKhoans");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.Ban", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.KhuVucBan", "KhuVucBan")
                        .WithMany("Bans")
                        .HasForeignKey("KhuVucBanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KhuVucBan");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.DatBan", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.Ban", null)
                        .WithMany("DatBans")
                        .HasForeignKey("BanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuanLyNhaHang_DATN.Models.KhachHang", "KhachHang")
                        .WithMany()
                        .HasForeignKey("KhachHangId");

                    b.HasOne("QuanLyNhaHang_DATN.Models.NhanVien", "NhanVien")
                        .WithMany()
                        .HasForeignKey("NhanVienId");

                    b.Navigation("KhachHang");

                    b.Navigation("NhanVien");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.GoiMon", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.DatBan", "DatBan")
                        .WithMany("GoiMons")
                        .HasForeignKey("DatBanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuanLyNhaHang_DATN.Models.MonAn", "MonAn")
                        .WithMany("GoiMons")
                        .HasForeignKey("MonAnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DatBan");

                    b.Navigation("MonAn");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.HoaDon", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.DatBan", "DatBan")
                        .WithMany()
                        .HasForeignKey("DatBanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuanLyNhaHang_DATN.Models.TaiKhoan", "TaiKhoan")
                        .WithMany("HoaDons")
                        .HasForeignKey("TaiKhoanId");

                    b.Navigation("DatBan");

                    b.Navigation("TaiKhoan");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.KhachHang", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.TaiKhoan", "TaiKhoan")
                        .WithMany()
                        .HasForeignKey("TaiKhoanId");

                    b.Navigation("TaiKhoan");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.MonAn", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.DanhMuc", "DanhMuc")
                        .WithMany("MonAns")
                        .HasForeignKey("DanhMucId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DanhMuc");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.NhanVien", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.TaiKhoan", "TaiKhoan")
                        .WithMany()
                        .HasForeignKey("TaiKhoanId");

                    b.Navigation("TaiKhoan");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.Quyen_ChucNang", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.ChucNang", "ChucNang")
                        .WithMany("Quyen_ChucNangs")
                        .HasForeignKey("ChucNangId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuanLyNhaHang_DATN.Models.Quyen", "Quyen")
                        .WithMany("Quyen_ChucNangs")
                        .HasForeignKey("QuyenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChucNang");

                    b.Navigation("Quyen");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.TaiKhoan", b =>
                {
                    b.HasOne("QuanLyNhaHang_DATN.Models.Quyen", "Quyen")
                        .WithMany()
                        .HasForeignKey("QuyenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Quyen");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.Ban", b =>
                {
                    b.Navigation("DatBans");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.ChucNang", b =>
                {
                    b.Navigation("Quyen_ChucNangs");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.DanhMuc", b =>
                {
                    b.Navigation("MonAns");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.DatBan", b =>
                {
                    b.Navigation("GoiMons");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.KhuVucBan", b =>
                {
                    b.Navigation("Bans");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.MonAn", b =>
                {
                    b.Navigation("GoiMons");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.Quyen", b =>
                {
                    b.Navigation("Quyen_ChucNangs");
                });

            modelBuilder.Entity("QuanLyNhaHang_DATN.Models.TaiKhoan", b =>
                {
                    b.Navigation("HoaDons");
                });
#pragma warning restore 612, 618
        }
    }
}
