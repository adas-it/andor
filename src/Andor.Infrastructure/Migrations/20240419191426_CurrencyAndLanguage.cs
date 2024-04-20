using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyAndLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                schema: "Onboarding",
                table: "Registration",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                schema: "Onboarding",
                table: "Registration",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LanguageId",
                schema: "Onboarding",
                table: "Registration",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "Onboarding",
                table: "Registration",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_CurrencyId",
                schema: "Onboarding",
                table: "Registration",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_LanguageId",
                schema: "Onboarding",
                table: "Registration",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Currency_CurrencyId",
                schema: "Onboarding",
                table: "Registration",
                column: "CurrencyId",
                principalSchema: "Engagement",
                principalTable: "Currency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Language_LanguageId",
                schema: "Onboarding",
                table: "Registration",
                column: "LanguageId",
                principalSchema: "Administration",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Currency_CurrencyId",
                schema: "Onboarding",
                table: "Registration");

            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Language_LanguageId",
                schema: "Onboarding",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_CurrencyId",
                schema: "Onboarding",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_LanguageId",
                schema: "Onboarding",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CountryId",
                schema: "Onboarding",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "Onboarding",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                schema: "Onboarding",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "Onboarding",
                table: "Registration");
        }
    }
}
