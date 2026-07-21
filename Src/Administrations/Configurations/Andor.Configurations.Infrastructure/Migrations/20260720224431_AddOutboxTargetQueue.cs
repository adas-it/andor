using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Configurations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxTargetQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetQueue",
                schema: "Outbox",
                table: "OutboxMessages",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetQueue",
                schema: "Outbox",
                table: "OutboxMessages");
        }
    }
}
