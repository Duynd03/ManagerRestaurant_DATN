using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang_DATN.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Model_ChucNang_and_ChucNangQuyen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quyen_ChucNang");

            migrationBuilder.DropTable(
                name: "ChucNang");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChucNang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenChucNang = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucNang", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quyen_ChucNang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChucNangId = table.Column<int>(type: "int", nullable: false),
                    QuyenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quyen_ChucNang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quyen_ChucNang_ChucNang_ChucNangId",
                        column: x => x.ChucNangId,
                        principalTable: "ChucNang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quyen_ChucNang_Quyen_QuyenId",
                        column: x => x.QuyenId,
                        principalTable: "Quyen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quyen_ChucNang_ChucNangId",
                table: "Quyen_ChucNang",
                column: "ChucNangId");

            migrationBuilder.CreateIndex(
                name: "IX_Quyen_ChucNang_QuyenId",
                table: "Quyen_ChucNang",
                column: "QuyenId");
        }
    }
}
