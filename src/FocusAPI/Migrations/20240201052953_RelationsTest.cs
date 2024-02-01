using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FocusAPI.Migrations
{
    /// <inheritdoc />
    public partial class RelationsTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Badges_UserBadges_UserBadgesId",
                table: "Badges");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_UserPets_UserPetsId",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserFriends_UserFriendsId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserFriends");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserFriendsId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Pets_UserPetsId",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Badges_UserBadgesId",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "UserFriendsId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserPetsId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "UserBadgesId",
                table: "Badges");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateCreated",
                table: "Users",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "PetId",
                table: "UserPets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BadgeId",
                table: "UserBadges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateAcquired",
                table: "UserBadges",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FriendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => new { x.UserId, x.FriendId, x.Status });
                    table.ForeignKey(
                        name: "FK_Friends_Users_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friends_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionHistory_UserId",
                table: "UserSessionHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPets_PetId",
                table: "UserPets",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPets_UserId",
                table: "UserPets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_UserId",
                table: "UserBadges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FriendId",
                table: "Friends",
                column: "FriendId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges",
                column: "BadgeId",
                principalTable: "Badges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPets_Pets_PetId",
                table: "UserPets",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPets_Users_UserId",
                table: "UserPets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionHistory_Users_UserId",
                table: "UserSessionHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Badges_BadgeId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBadges_Users_UserId",
                table: "UserBadges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPets_Pets_PetId",
                table: "UserPets");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPets_Users_UserId",
                table: "UserPets");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionHistory_Users_UserId",
                table: "UserSessionHistory");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_UserSessionHistory_UserId",
                table: "UserSessionHistory");

            migrationBuilder.DropIndex(
                name: "IX_UserPets_PetId",
                table: "UserPets");

            migrationBuilder.DropIndex(
                name: "IX_UserPets_UserId",
                table: "UserPets");

            migrationBuilder.DropIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges");

            migrationBuilder.DropIndex(
                name: "IX_UserBadges_UserId",
                table: "UserBadges");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "UserPets");

            migrationBuilder.DropColumn(
                name: "BadgeId",
                table: "UserBadges");

            migrationBuilder.DropColumn(
                name: "DateAcquired",
                table: "UserBadges");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddColumn<Guid>(
                name: "UserFriendsId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserPetsId",
                table: "Pets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserBadgesId",
                table: "Badges",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFriends",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriends", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserFriendsId",
                table: "Users",
                column: "UserFriendsId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_UserPetsId",
                table: "Pets",
                column: "UserPetsId");

            migrationBuilder.CreateIndex(
                name: "IX_Badges_UserBadgesId",
                table: "Badges",
                column: "UserBadgesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Badges_UserBadges_UserBadgesId",
                table: "Badges",
                column: "UserBadgesId",
                principalTable: "UserBadges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_UserPets_UserPetsId",
                table: "Pets",
                column: "UserPetsId",
                principalTable: "UserPets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserFriends_UserFriendsId",
                table: "Users",
                column: "UserFriendsId",
                principalTable: "UserFriends",
                principalColumn: "Id");
        }
    }
}
