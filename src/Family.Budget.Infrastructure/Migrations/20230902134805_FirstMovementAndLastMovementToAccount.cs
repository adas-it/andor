using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family.Budget.Infrastructure.Migrations
{
    public partial class FirstMovementAndLastMovementToAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstMovement",
                schema: "budget",
                table: "Account",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMovement",
                schema: "budget",
                table: "Account",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstMovement",
                schema: "budget",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LastMovement",
                schema: "budget",
                table: "Account");
        }
    }
}
