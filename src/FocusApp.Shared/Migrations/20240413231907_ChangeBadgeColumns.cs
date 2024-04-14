using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBadgeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Badges");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Badges",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Badges");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Badges",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
