using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RenameFurnitureToDecor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Furniture_SelectedFurnitureId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserFurniture");

            migrationBuilder.DropTable(
                name: "Furniture");

            migrationBuilder.RenameColumn(
                name: "SelectedFurnitureId",
                table: "Users",
                newName: "SelectedDecorId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_SelectedFurnitureId",
                table: "Users",
                newName: "IX_Users_SelectedDecorId");

            migrationBuilder.CreateTable(
                name: "Decor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: false),
                    HeightRequest = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDecor",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DecorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateAcquired = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDecor", x => new { x.UserId, x.DecorId });
                    table.ForeignKey(
                        name: "FK_UserDecor_Decor_DecorId",
                        column: x => x.DecorId,
                        principalTable: "Decor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDecor_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDecor_DecorId",
                table: "UserDecor",
                column: "DecorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Decor_SelectedDecorId",
                table: "Users",
                column: "SelectedDecorId",
                principalTable: "Decor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Decor_SelectedDecorId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserDecor");

            migrationBuilder.DropTable(
                name: "Decor");

            migrationBuilder.RenameColumn(
                name: "SelectedDecorId",
                table: "Users",
                newName: "SelectedFurnitureId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_SelectedDecorId",
                table: "Users",
                newName: "IX_Users_SelectedFurnitureId");

            migrationBuilder.CreateTable(
                name: "Furniture",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HeightRequest = table.Column<int>(type: "INTEGER", nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Furniture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFurniture",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FurnitureId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateAcquired = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFurniture", x => new { x.UserId, x.FurnitureId });
                    table.ForeignKey(
                        name: "FK_UserFurniture_Furniture_FurnitureId",
                        column: x => x.FurnitureId,
                        principalTable: "Furniture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFurniture_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFurniture_FurnitureId",
                table: "UserFurniture",
                column: "FurnitureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Furniture_SelectedFurnitureId",
                table: "Users",
                column: "SelectedFurnitureId",
                principalTable: "Furniture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
