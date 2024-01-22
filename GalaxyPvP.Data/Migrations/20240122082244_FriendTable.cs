using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GalaxyPvP.Data.Migrations
{
    /// <inheritdoc />
    public partial class FriendTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Player1 = table.Column<int>(type: "int", nullable: false),
                    Player2 = table.Column<int>(type: "int", nullable: false),
                    state = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.Id);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DeleteData(
                table: "GameConfigs",
                keyColumn: "Key",
                keyValue: "Version_Android");

            migrationBuilder.DeleteData(
                table: "GameConfigs",
                keyColumn: "Key",
                keyValue: "Version_iOS");

            migrationBuilder.DeleteData(
                table: "GameConfigs",
                keyColumn: "Key",
                keyValue: "Version_Windows");

        }
    }
}
