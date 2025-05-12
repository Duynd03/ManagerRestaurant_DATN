using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Create_BanSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BanSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BanId = table.Column<int>(type: "int", nullable: false),
                    DatBanId = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BanSchedule_Ban_BanId",
                        column: x => x.BanId,
                        principalTable: "Ban",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BanSchedule_DatBan_DatBanId",
                        column: x => x.DatBanId,
                        principalTable: "DatBan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BanSchedule_BanId",
                table: "BanSchedule",
                column: "BanId");

            migrationBuilder.CreateIndex(
                name: "IX_BanSchedule_DatBanId",
                table: "BanSchedule",
                column: "DatBanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BanSchedule");
        }
    }
}
