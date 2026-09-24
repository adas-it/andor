using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRecipientPreferredLanguageToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                schema: "Communication",
                table: "Recipient");

            migrationBuilder.AddColumn<Guid>(
                name: "PreferredLanguageId",
                schema: "Communication",
                table: "Recipient",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredLanguageId",
                schema: "Communication",
                table: "Recipient");

            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                schema: "Communication",
                table: "Recipient",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
