using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Onboarding.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSignupRequestPreferredCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredCurrency",
                schema: "Onboarding",
                table: "SignupRequest",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredCurrency",
                schema: "Onboarding",
                table: "SignupRequest");
        }
    }
}
