using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusApp.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedItemsToUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SelectedBadgeId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedFurnitureId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedIslandId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedPetId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SelectedBadgeId",
                table: "Users",
                column: "SelectedBadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SelectedFurnitureId",
                table: "Users",
                column: "SelectedFurnitureId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SelectedIslandId",
                table: "Users",
                column: "SelectedIslandId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SelectedPetId",
                table: "Users",
                column: "SelectedPetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Badges_SelectedBadgeId",
                table: "Users",
                column: "SelectedBadgeId",
                principalTable: "Badges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Furniture_SelectedFurnitureId",
                table: "Users",
                column: "SelectedFurnitureId",
                principalTable: "Furniture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Islands_SelectedIslandId",
                table: "Users",
                column: "SelectedIslandId",
                principalTable: "Islands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Pets_SelectedPetId",
                table: "Users",
                column: "SelectedPetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Badges_SelectedBadgeId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Furniture_SelectedFurnitureId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Islands_SelectedIslandId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Pets_SelectedPetId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SelectedBadgeId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SelectedFurnitureId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SelectedIslandId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SelectedPetId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedBadgeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedFurnitureId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedIslandId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedPetId",
                table: "Users");
        }
    }
}
