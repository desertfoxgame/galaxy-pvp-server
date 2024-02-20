using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalaxyPvP.Data.Migrations
{
    /// <inheritdoc />
    public partial class PlayerUserKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Player_UserId",
                table: "Player",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Player_AspNetUsers_UserId",
                table: "Player",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Player_AspNetUsers_UserId",
                table: "Player");

            migrationBuilder.DropIndex(
                name: "IX_Player_UserId",
                table: "Player");
        }
    }
}
