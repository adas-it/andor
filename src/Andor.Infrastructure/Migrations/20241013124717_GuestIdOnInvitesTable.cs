using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GuestIdOnInvitesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GuestId",
                schema: "Engagement",
                table: "Invite",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvitingId",
                schema: "Engagement",
                table: "Invite",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "InviteSagaState",
                schema: "Engagement",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    InviteId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuestId = table.Column<Guid>(type: "uuid", nullable: true),
                    GuestEmail = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteSagaState", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Engagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invite_GuestId",
                schema: "Engagement",
                table: "Invite",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Invite_InvitingId",
                schema: "Engagement",
                table: "Invite",
                column: "InvitingId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountUser_UserId",
                schema: "Engagement",
                table: "AccountUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountUser_User_UserId",
                schema: "Engagement",
                table: "AccountUser",
                column: "UserId",
                principalSchema: "Engagement",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invite_User_GuestId",
                schema: "Engagement",
                table: "Invite",
                column: "GuestId",
                principalSchema: "Engagement",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invite_User_InvitingId",
                schema: "Engagement",
                table: "Invite",
                column: "InvitingId",
                principalSchema: "Engagement",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountUser_User_UserId",
                schema: "Engagement",
                table: "AccountUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Invite_User_GuestId",
                schema: "Engagement",
                table: "Invite");

            migrationBuilder.DropForeignKey(
                name: "FK_Invite_User_InvitingId",
                schema: "Engagement",
                table: "Invite");

            migrationBuilder.DropTable(
                name: "InviteSagaState",
                schema: "Engagement");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Engagement");

            migrationBuilder.DropIndex(
                name: "IX_Invite_GuestId",
                schema: "Engagement",
                table: "Invite");

            migrationBuilder.DropIndex(
                name: "IX_Invite_InvitingId",
                schema: "Engagement",
                table: "Invite");

            migrationBuilder.DropIndex(
                name: "IX_AccountUser_UserId",
                schema: "Engagement",
                table: "AccountUser");

            migrationBuilder.DropColumn(
                name: "GuestId",
                schema: "Engagement",
                table: "Invite");

            migrationBuilder.DropColumn(
                name: "InvitingId",
                schema: "Engagement",
                table: "Invite");
        }
    }
}
