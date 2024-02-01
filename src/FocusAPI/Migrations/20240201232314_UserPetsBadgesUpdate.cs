using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserPetsBadgesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPets_Users_UserId",
                table: "UserPets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPets",
                table: "UserPets");

            migrationBuilder.DropIndex(
                name: "IX_UserPets_UserId",
                table: "UserPets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges");

            migrationBuilder.DropIndex(
                name: "IX_UserBadges_UserId",
                table: "UserBadges");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserBadges");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPets",
                table: "UserPets",
                columns: new[] { "UserId", "PetId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges",
                columns: new[] { "UserId", "BadgeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPets_Users_UserId",
                table: "UserPets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPets_Users_UserId",
                table: "UserPets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPets",
                table: "UserPets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserPets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UserBadges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPets",
                table: "UserPets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBadges",
                table: "UserBadges",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserPets_UserId",
                table: "UserPets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_UserId",
                table: "UserBadges",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPets_Users_UserId",
                table: "UserPets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
