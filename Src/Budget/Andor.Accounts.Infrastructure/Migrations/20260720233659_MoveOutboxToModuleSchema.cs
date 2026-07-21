using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveOutboxToModuleSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AccountsOutbox");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "Outbox",
                newName: "OutboxMessages",
                newSchema: "AccountsOutbox");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Outbox");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "AccountsOutbox",
                newName: "OutboxMessages",
                newSchema: "Outbox");
        }
    }
}
