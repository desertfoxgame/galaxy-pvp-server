using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalaxyPvP.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatecol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "VerifyCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmTermsOfService",
                table: "Player",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Verification",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "VerifyCodes");

            migrationBuilder.DropColumn(
                name: "ConfirmTermsOfService",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Verification",
                table: "AspNetUsers");
        }
    }
}
