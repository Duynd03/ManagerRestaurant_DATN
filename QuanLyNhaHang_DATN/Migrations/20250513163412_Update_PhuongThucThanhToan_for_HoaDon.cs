using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update_PhuongThucThanhToan_for_HoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhuongThucThanhToan",
                table: "HoaDon");

            migrationBuilder.AddColumn<int>(
                name: "PhuongThuc",
                table: "HoaDon",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhuongThuc",
                table: "HoaDon");

            migrationBuilder.AddColumn<int>(
                name: "PhuongThucThanhToan",
                table: "HoaDon",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
