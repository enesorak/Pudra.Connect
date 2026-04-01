using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pudra.Connect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImageAndScanCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScanCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ScanLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Found = table.Column<bool>(type: "bit", nullable: false),
                    ScannedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScanLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScanLogs_Barcode",
                table: "ScanLogs",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_ScanLogs_ScannedAt",
                table: "ScanLogs",
                column: "ScannedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ScanLogs_UserId",
                table: "ScanLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScanLogs");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ScanCount",
                table: "Users");
        }
    }
}
