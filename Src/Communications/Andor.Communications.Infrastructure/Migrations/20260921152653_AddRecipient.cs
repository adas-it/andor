using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipient",
                schema: "Communication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    MarketingOptIn = table.Column<bool>(type: "bit", nullable: false),
                    TermsAndConditionsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyPolicyAccepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipient", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipient",
                schema: "Communication");
        }
    }
}
