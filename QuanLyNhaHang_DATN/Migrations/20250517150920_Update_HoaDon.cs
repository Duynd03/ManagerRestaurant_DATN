using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update_HoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NhanVienId",
                table: "HoaDon",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_NhanVienId",
                table: "HoaDon",
                column: "NhanVienId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_NhanVien_NhanVienId",
                table: "HoaDon",
                column: "NhanVienId",
                principalTable: "NhanVien",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_NhanVien_NhanVienId",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_NhanVienId",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "NhanVienId",
                table: "HoaDon");
        }
    }
}
