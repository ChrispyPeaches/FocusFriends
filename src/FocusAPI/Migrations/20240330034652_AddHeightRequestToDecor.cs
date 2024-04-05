using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHeightRequestToDecor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeightRequest",
                table: "Furniture",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeightRequest",
                table: "Furniture");
        }
    }
}
