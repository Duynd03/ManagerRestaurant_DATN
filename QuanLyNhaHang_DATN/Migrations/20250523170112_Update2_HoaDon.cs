using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update2_HoaDon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Đổi tên cột TongTien thành TongTienGoiMon
            migrationBuilder.RenameColumn(
                name: "TongTien",
                table: "HoaDon",
                newName: "TongTienGoiMon");

            // Thêm cột TongTienThanhToan (không nullable, mặc định là 0m)
            migrationBuilder.AddColumn<decimal>(
                name: "TongTienThanhToan",
                table: "HoaDon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
           

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Đổi tên ngược lại: TongTienGoiMon thành TongTien
            migrationBuilder.RenameColumn(
                name: "TongTienGoiMon",
                table: "HoaDon",
                newName: "TongTien");

            // Xóa cột TongTienThanhToan
            migrationBuilder.DropColumn(
                name: "TongTienThanhToan",
                table: "HoaDon");

            
        }
    }
}
