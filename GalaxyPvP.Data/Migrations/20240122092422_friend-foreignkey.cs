using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalaxyPvP.Data.Migrations
{
    /// <inheritdoc />
    public partial class friendforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "Player2",
                table: "Friends");

            migrationBuilder.AddColumn<string>(
                name: "Player1Id",
                table: "Friends",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Player2Id",
                table: "Friends",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_Player1Id",
                table: "Friends",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_Player2Id",
                table: "Friends",
                column: "Player2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Player_Player1Id",
                table: "Friends",
                column: "Player1Id",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Player_Player2Id",
                table: "Friends",
                column: "Player2Id",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Player_Player1Id",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Player_Player2Id",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_Player1Id",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_Player2Id",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "Player1Id",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "Player2Id",
                table: "Friends");

            migrationBuilder.AddColumn<int>(
                name: "Player1",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
