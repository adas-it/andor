using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Configurations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveOutboxToModuleSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ConfigurationsOutbox");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "Outbox",
                newName: "OutboxMessages",
                newSchema: "ConfigurationsOutbox");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Outbox");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "ConfigurationsOutbox",
                newName: "OutboxMessages",
                newSchema: "Outbox");
        }
    }
}
