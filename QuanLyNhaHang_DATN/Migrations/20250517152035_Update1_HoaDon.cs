using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update1_HoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa cột TaiKhoanId
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_TaiKhoan_TaiKhoanId",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_TaiKhoanId",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "TaiKhoanId",
                table: "HoaDon");

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Thêm lại cột TaiKhoanId
            migrationBuilder.AddColumn<int>(
                name: "TaiKhoanId",
                table: "HoaDon",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_TaiKhoanId",
                table: "HoaDon",
                column: "TaiKhoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_TaiKhoan_TaiKhoanId",
                table: "HoaDon",
                column: "TaiKhoanId",
                principalTable: "TaiKhoan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

        }
    }
}
