using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Update_4_DatBanBans_DatBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan");

            migrationBuilder.DropIndex(
                name: "IX_DatBan_BanId",
                table: "DatBan");

            migrationBuilder.DropColumn(
                name: "BanId",
                table: "DatBan");

            migrationBuilder.CreateTable(
                name: "DatBan_Ban",
                columns: table => new
                {
                    DatBanId = table.Column<int>(type: "int", nullable: false),
                    BanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatBan_Ban", x => new { x.DatBanId, x.BanId });
                    table.ForeignKey(
                        name: "FK_DatBan_Ban_Ban_BanId",
                        column: x => x.BanId,
                        principalTable: "Ban",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatBan_Ban_DatBan_DatBanId",
                        column: x => x.DatBanId,
                        principalTable: "DatBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_Ban_BanId",
                table: "DatBan_Ban",
                column: "BanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatBan_Ban");

            migrationBuilder.AddColumn<int>(
                name: "BanId",
                table: "DatBan",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatBan_BanId",
                table: "DatBan",
                column: "BanId");

            migrationBuilder.AddForeignKey(
                name: "FK_DatBan_Ban_BanId",
                table: "DatBan",
                column: "BanId",
                principalTable: "Ban",
                principalColumn: "Id");
        }
    }
}
