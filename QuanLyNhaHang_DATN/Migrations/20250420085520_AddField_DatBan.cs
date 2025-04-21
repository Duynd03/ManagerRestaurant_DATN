using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class AddField_DatBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDatHo",
                table: "DatBans",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SDTLienHe",
                table: "DatBans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenLienHe",
                table: "DatBans",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDatHo",
                table: "DatBans");

            migrationBuilder.DropColumn(
                name: "SDTLienHe",
                table: "DatBans");

            migrationBuilder.DropColumn(
                name: "TenLienHe",
                table: "DatBans");
        }
    }
}
