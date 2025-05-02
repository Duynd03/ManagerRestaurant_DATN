using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update_DB_Identity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bans_KhuVucBans_KhuVucBanId",
                table: "Bans");

            migrationBuilder.DropForeignKey(
                name: "FK_DatBans_Bans_BanId",
                table: "DatBans");

            migrationBuilder.DropForeignKey(
                name: "FK_DatBans_KhachHangs_KhachHangId",
                table: "DatBans");

            migrationBuilder.DropForeignKey(
                name: "FK_DatBans_NhanViens_NhanVienId",
                table: "DatBans");

            migrationBuilder.DropForeignKey(
                name: "FK_GoiMons_DatBans_DatBanId",
                table: "GoiMons");

            migrationBuilder.DropForeignKey(
                name: "FK_GoiMons_MonAns_MonAnId",
                table: "GoiMons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_DatBans_DatBanId",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_TaiKhoans_TaiKhoanId",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangs_TaiKhoans_TaiKhoanId",
                table: "KhachHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_MonAns_DanhMucs_DanhMucId",
                table: "MonAns");

            migrationBuilder.DropForeignKey(
                name: "FK_NhanViens_TaiKhoans_TaiKhoanId",
                table: "NhanViens");

            migrationBuilder.DropForeignKey(
                name: "FK_Quyen_ChucNangs_ChucNangs_ChucNangId",
                table: "Quyen_ChucNangs");

            migrationBuilder.DropForeignKey(
                name: "FK_Quyen_ChucNangs_Quyens_QuyenId",
                table: "Quyen_ChucNangs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoans_Quyens_QuyenId",
                table: "TaiKhoans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaiKhoans",
                table: "TaiKhoans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quyens",
                table: "Quyens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quyen_ChucNangs",
                table: "Quyen_ChucNangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhanViens",
                table: "NhanViens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonAns",
                table: "MonAns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhuVucBans",
                table: "KhuVucBans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhachHangs",
                table: "KhachHangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HoaDons",
                table: "HoaDons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoiMons",
                table: "GoiMons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DatBans",
                table: "DatBans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMucs",
                table: "DanhMucs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChucNangs",
                table: "ChucNangs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bans",
                table: "Bans");

            migrationBuilder.DropColumn(
                name: "MatKhau",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "TenTaiKhoan",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "TenQuyen",
                table: "Quyens");

            migrationBuilder.RenameTable(
                name: "TaiKhoans",
                newName: "TaiKhoan");

            migrationBuilder.RenameTable(
                name: "Quyens",
                newName: "Quyen");

            migrationBuilder.RenameTable(
                name: "Quyen_ChucNangs",
                newName: "Quyen_ChucNang");

            migrationBuilder.RenameTable(
                name: "NhanViens",
                newName: "NhanVien");

            migrationBuilder.RenameTable(
                name: "MonAns",
                newName: "MonAn");

            migrationBuilder.RenameTable(
                name: "KhuVucBans",
                newName: "KhuVucBan");

            migrationBuilder.RenameTable(
                name: "KhachHangs",
                newName: "KhachHang");

            migrationBuilder.RenameTable(
                name: "HoaDons",
                newName: "HoaDon");

            migrationBuilder.RenameTable(
                name: "GoiMons",
                newName: "GoiMon");

            migrationBuilder.RenameTable(
                name: "DatBans",
                newName: "DatBan");

            migrationBuilder.RenameTable(
                name: "DanhMucs",
                newName: "DanhMuc");

            migrationBuilder.RenameTable(
                name: "ChucNangs",
                newName: "ChucNang");

            migrationBuilder.RenameTable(
                name: "Bans",
                newName: "Ban");

            migrationBuilder.RenameIndex(
                name: "IX_TaiKhoans_QuyenId",
                table: "TaiKhoan",
                newName: "IX_TaiKhoan_QuyenId");

            migrationBuilder.RenameIndex(
                name: "IX_Quyen_ChucNangs_QuyenId",
                table: "Quyen_ChucNang",
                newName: "IX_Quyen_ChucNang_QuyenId");

            migrationBuilder.RenameIndex(
                name: "IX_Quyen_ChucNangs_ChucNangId",
                table: "Quyen_ChucNang",
                newName: "IX_Quyen_ChucNang_ChucNangId");

            migrationBuilder.RenameIndex(
                name: "IX_NhanViens_TaiKhoanId",
                table: "NhanVien",
                newName: "IX_NhanVien_TaiKhoanId");

            migrationBuilder.RenameIndex(
                name: "IX_MonAns_DanhMucId",
                table: "MonAn",
                newName: "IX_MonAn_DanhMucId");

            migrationBuilder.RenameIndex(
                name: "IX_KhachHangs_TaiKhoanId",
                table: "KhachHang",
                newName: "IX_KhachHang_TaiKhoanId");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDons_TaiKhoanId",
                table: "HoaDon",
                newName: "IX_HoaDon_TaiKhoanId");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDons_DatBanId",
                table: "HoaDon",
                newName: "IX_HoaDon_DatBanId");

            migrationBuilder.RenameIndex(
                name: "IX_GoiMons_MonAnId",
                table: "GoiMon",
                newName: "IX_GoiMon_MonAnId");

            migrationBuilder.RenameIndex(
                name: "IX_GoiMons_DatBanId",
                table: "GoiMon",
                newName: "IX_GoiMon_DatBanId");

            migrationBuilder.RenameIndex(
                name: "IX_DatBans_NhanVienId",
                table: "DatBan",
                newName: "IX_DatBan_NhanVienId");

            migrationBuilder.RenameIndex(
                name: "IX_DatBans_KhachHangId",
                table: "DatBan",
                newName: "IX_DatBan_KhachHangId");

            migrationBuilder.RenameIndex(
                name: "IX_DatBans_BanId",
                table: "DatBan",
                newName: "IX_DatBan_BanId");

            migrationBuilder.RenameIndex(
                name: "IX_Bans_KhuVucBanId",
                table: "Ban",
                newName: "IX_Ban_KhuVucBanId");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "TaiKhoan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "TaiKhoan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TaiKhoan",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "TaiKhoan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "TaiKhoan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "TaiKhoan",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "TaiKhoan",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "TaiKhoan",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "TaiKhoan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "TaiKhoan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "TaiKhoan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "TaiKhoan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "TaiKhoan",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "TaiKhoan",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Quyen",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Quyen",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Quyen",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaiKhoan",
                table: "TaiKhoan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quyen",
                table: "Quyen",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quyen_ChucNang",
                table: "Quyen_ChucNang",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhanVien",
                table: "NhanVien",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonAn",
                table: "MonAn",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhuVucBan",
                table: "KhuVucBan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhachHang",
                table: "KhachHang",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HoaDon",
                table: "HoaDon",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoiMon",
                table: "GoiMon",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatBan",
                table: "DatBan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMuc",
                table: "DanhMuc",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChucNang",
                table: "ChucNang",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ban",
                table: "Ban",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Quyen_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Quyen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_TaiKhoan_UserId",
                        column: x => x.UserId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_TaiKhoan_UserId",
                        column: x => x.UserId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Quyen_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Quyen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_TaiKhoan_UserId",
                        column: x => x.UserId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_TaiKhoan_UserId",
                        column: x => x.UserId,
                        principalTable: "TaiKhoan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "TaiKhoan",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "TaiKhoan",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Quyen",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ban_KhuVucBan_KhuVucBanId",
                table: "Ban",
                column: "KhuVucBanId",
                principalTable: "KhuVucBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan",
                column: "BanId",
                principalTable: "Ban",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DatBan_KhachHang_KhachHangId",
                table: "DatBan",
                column: "KhachHangId",
                principalTable: "KhachHang",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DatBan_NhanVien_NhanVienId",
                table: "DatBan",
                column: "NhanVienId",
                principalTable: "NhanVien",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoiMon_DatBan_DatBanId",
                table: "GoiMon",
                column: "DatBanId",
                principalTable: "DatBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GoiMon_MonAn_MonAnId",
                table: "GoiMon",
                column: "MonAnId",
                principalTable: "MonAn",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_DatBan_DatBanId",
                table: "HoaDon",
                column: "DatBanId",
                principalTable: "DatBan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_TaiKhoan_TaiKhoanId",
                table: "HoaDon",
                column: "TaiKhoanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHang_TaiKhoan_TaiKhoanId",
                table: "KhachHang",
                column: "TaiKhoanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonAn_DanhMuc_DanhMucId",
                table: "MonAn",
                column: "DanhMucId",
                principalTable: "DanhMuc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NhanVien_TaiKhoan_TaiKhoanId",
                table: "NhanVien",
                column: "TaiKhoanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quyen_ChucNang_ChucNang_ChucNangId",
                table: "Quyen_ChucNang",
                column: "ChucNangId",
                principalTable: "ChucNang",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quyen_ChucNang_Quyen_QuyenId",
                table: "Quyen_ChucNang",
                column: "QuyenId",
                principalTable: "Quyen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoan_Quyen_QuyenId",
                table: "TaiKhoan",
                column: "QuyenId",
                principalTable: "Quyen",
                principalColumn: "Id",
                //onDelete: ReferentialAction.Cascade
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ban_KhuVucBan_KhuVucBanId",
                table: "Ban");

            migrationBuilder.DropForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan");

            migrationBuilder.DropForeignKey(
                name: "FK_DatBan_KhachHang_KhachHangId",
                table: "DatBan");

            migrationBuilder.DropForeignKey(
                name: "FK_DatBan_NhanVien_NhanVienId",
                table: "DatBan");

            migrationBuilder.DropForeignKey(
                name: "FK_GoiMon_DatBan_DatBanId",
                table: "GoiMon");

            migrationBuilder.DropForeignKey(
                name: "FK_GoiMon_MonAn_MonAnId",
                table: "GoiMon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_DatBan_DatBanId",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_TaiKhoan_TaiKhoanId",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_KhachHang_TaiKhoan_TaiKhoanId",
                table: "KhachHang");

            migrationBuilder.DropForeignKey(
                name: "FK_MonAn_DanhMuc_DanhMucId",
                table: "MonAn");

            migrationBuilder.DropForeignKey(
                name: "FK_NhanVien_TaiKhoan_TaiKhoanId",
                table: "NhanVien");

            migrationBuilder.DropForeignKey(
                name: "FK_Quyen_ChucNang_ChucNang_ChucNangId",
                table: "Quyen_ChucNang");

            migrationBuilder.DropForeignKey(
                name: "FK_Quyen_ChucNang_Quyen_QuyenId",
                table: "Quyen_ChucNang");

            migrationBuilder.DropForeignKey(
                name: "FK_TaiKhoan_Quyen_QuyenId",
                table: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaiKhoan",
                table: "TaiKhoan");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "TaiKhoan");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "TaiKhoan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quyen_ChucNang",
                table: "Quyen_ChucNang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quyen",
                table: "Quyen");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Quyen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NhanVien",
                table: "NhanVien");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MonAn",
                table: "MonAn");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhuVucBan",
                table: "KhuVucBan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhachHang",
                table: "KhachHang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HoaDon",
                table: "HoaDon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoiMon",
                table: "GoiMon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DatBan",
                table: "DatBan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMuc",
                table: "DanhMuc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChucNang",
                table: "ChucNang");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ban",
                table: "Ban");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Quyen");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Quyen");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Quyen");

            migrationBuilder.RenameTable(
                name: "TaiKhoan",
                newName: "TaiKhoans");

            migrationBuilder.RenameTable(
                name: "Quyen_ChucNang",
                newName: "Quyen_ChucNangs");

            migrationBuilder.RenameTable(
                name: "Quyen",
                newName: "Quyens");

            migrationBuilder.RenameTable(
                name: "NhanVien",
                newName: "NhanViens");

            migrationBuilder.RenameTable(
                name: "MonAn",
                newName: "MonAns");

            migrationBuilder.RenameTable(
                name: "KhuVucBan",
                newName: "KhuVucBans");

            migrationBuilder.RenameTable(
                name: "KhachHang",
                newName: "KhachHangs");

            migrationBuilder.RenameTable(
                name: "HoaDon",
                newName: "HoaDons");

            migrationBuilder.RenameTable(
                name: "GoiMon",
                newName: "GoiMons");

            migrationBuilder.RenameTable(
                name: "DatBan",
                newName: "DatBans");

            migrationBuilder.RenameTable(
                name: "DanhMuc",
                newName: "DanhMucs");

            migrationBuilder.RenameTable(
                name: "ChucNang",
                newName: "ChucNangs");

            migrationBuilder.RenameTable(
                name: "Ban",
                newName: "Bans");

            migrationBuilder.RenameIndex(
                name: "IX_TaiKhoan_QuyenId",
                table: "TaiKhoans",
                newName: "IX_TaiKhoans_QuyenId");

            migrationBuilder.RenameIndex(
                name: "IX_Quyen_ChucNang_QuyenId",
                table: "Quyen_ChucNangs",
                newName: "IX_Quyen_ChucNangs_QuyenId");

            migrationBuilder.RenameIndex(
                name: "IX_Quyen_ChucNang_ChucNangId",
                table: "Quyen_ChucNangs",
                newName: "IX_Quyen_ChucNangs_ChucNangId");

            migrationBuilder.RenameIndex(
                name: "IX_NhanVien_TaiKhoanId",
                table: "NhanViens",
                newName: "IX_NhanViens_TaiKhoanId");

            migrationBuilder.RenameIndex(
                name: "IX_MonAn_DanhMucId",
                table: "MonAns",
                newName: "IX_MonAns_DanhMucId");

            migrationBuilder.RenameIndex(
                name: "IX_KhachHang_TaiKhoanId",
                table: "KhachHangs",
                newName: "IX_KhachHangs_TaiKhoanId");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_TaiKhoanId",
                table: "HoaDons",
                newName: "IX_HoaDons_TaiKhoanId");

            migrationBuilder.RenameIndex(
                name: "IX_HoaDon_DatBanId",
                table: "HoaDons",
                newName: "IX_HoaDons_DatBanId");

            migrationBuilder.RenameIndex(
                name: "IX_GoiMon_MonAnId",
                table: "GoiMons",
                newName: "IX_GoiMons_MonAnId");

            migrationBuilder.RenameIndex(
                name: "IX_GoiMon_DatBanId",
                table: "GoiMons",
                newName: "IX_GoiMons_DatBanId");

            migrationBuilder.RenameIndex(
                name: "IX_DatBan_NhanVienId",
                table: "DatBans",
                newName: "IX_DatBans_NhanVienId");

            migrationBuilder.RenameIndex(
                name: "IX_DatBan_KhachHangId",
                table: "DatBans",
                newName: "IX_DatBans_KhachHangId");

            migrationBuilder.RenameIndex(
                name: "IX_DatBan_BanId",
                table: "DatBans",
                newName: "IX_DatBans_BanId");

            migrationBuilder.RenameIndex(
                name: "IX_Ban_KhuVucBanId",
                table: "Bans",
                newName: "IX_Bans_KhuVucBanId");

            migrationBuilder.AddColumn<string>(
                name: "MatKhau",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenTaiKhoan",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenQuyen",
                table: "Quyens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaiKhoans",
                table: "TaiKhoans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quyen_ChucNangs",
                table: "Quyen_ChucNangs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quyens",
                table: "Quyens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NhanViens",
                table: "NhanViens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonAns",
                table: "MonAns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhuVucBans",
                table: "KhuVucBans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhachHangs",
                table: "KhachHangs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HoaDons",
                table: "HoaDons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoiMons",
                table: "GoiMons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DatBans",
                table: "DatBans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMucs",
                table: "DanhMucs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChucNangs",
                table: "ChucNangs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bans",
                table: "Bans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bans_KhuVucBans_KhuVucBanId",
                table: "Bans",
                column: "KhuVucBanId",
                principalTable: "KhuVucBans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DatBans_Bans_BanId",
                table: "DatBans",
                column: "BanId",
                principalTable: "Bans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DatBans_KhachHangs_KhachHangId",
                table: "DatBans",
                column: "KhachHangId",
                principalTable: "KhachHangs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DatBans_NhanViens_NhanVienId",
                table: "DatBans",
                column: "NhanVienId",
                principalTable: "NhanViens",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoiMons_DatBans_DatBanId",
                table: "GoiMons",
                column: "DatBanId",
                principalTable: "DatBans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GoiMons_MonAns_MonAnId",
                table: "GoiMons",
                column: "MonAnId",
                principalTable: "MonAns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_DatBans_DatBanId",
                table: "HoaDons",
                column: "DatBanId",
                principalTable: "DatBans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_TaiKhoans_TaiKhoanId",
                table: "HoaDons",
                column: "TaiKhoanId",
                principalTable: "TaiKhoans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangs_TaiKhoans_TaiKhoanId",
                table: "KhachHangs",
                column: "TaiKhoanId",
                principalTable: "TaiKhoans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MonAns_DanhMucs_DanhMucId",
                table: "MonAns",
                column: "DanhMucId",
                principalTable: "DanhMucs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NhanViens_TaiKhoans_TaiKhoanId",
                table: "NhanViens",
                column: "TaiKhoanId",
                principalTable: "TaiKhoans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quyen_ChucNangs_ChucNangs_ChucNangId",
                table: "Quyen_ChucNangs",
                column: "ChucNangId",
                principalTable: "ChucNangs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quyen_ChucNangs_Quyens_QuyenId",
                table: "Quyen_ChucNangs",
                column: "QuyenId",
                principalTable: "Quyens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaiKhoans_Quyens_QuyenId",
                table: "TaiKhoans",
                column: "QuyenId",
                principalTable: "Quyens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
