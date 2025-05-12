using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update_Gia_MonAn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Gia",
                table: "MonAn",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Gia",
                table: "MonAn",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
