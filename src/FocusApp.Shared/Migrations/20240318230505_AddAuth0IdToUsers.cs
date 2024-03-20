using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddAuth0IdToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth0Id",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth0Id",
                table: "Users");
        }
    }
}
