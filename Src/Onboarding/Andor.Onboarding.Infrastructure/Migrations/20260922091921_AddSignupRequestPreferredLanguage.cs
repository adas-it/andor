using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Onboarding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSignupRequestPreferredLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                schema: "Onboarding",
                table: "SignupRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                schema: "Onboarding",
                table: "SignupRequest");
        }
    }
}
