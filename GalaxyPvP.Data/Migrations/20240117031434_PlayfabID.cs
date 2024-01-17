using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalaxyPvP.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlayfabID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayfabId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayfabId",
                table: "AspNetUsers");
        }
    }
}
