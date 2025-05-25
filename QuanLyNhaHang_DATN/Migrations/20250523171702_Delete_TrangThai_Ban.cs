using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Delete_TrangThai_Ban : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa cột TrangThai khỏi bảng Ban
            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "Ban");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Thêm lại cột TrangThai với kiểu int, không nullable, giá trị mặc định là 0
            migrationBuilder.AddColumn<int>(
                name: "TrangThai",
                table: "Ban",
                type: "int",
                nullable: false);
        }
    }
}
