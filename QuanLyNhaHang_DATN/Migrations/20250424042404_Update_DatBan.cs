using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update_DatBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan");

            migrationBuilder.AlterColumn<int>(
                name: "BanId",
                table: "DatBan",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan",
                column: "BanId",
                principalTable: "Ban",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan");

            migrationBuilder.AlterColumn<int>(
                name: "BanId",
                table: "DatBan",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan",
                column: "BanId",
                principalTable: "Ban",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
